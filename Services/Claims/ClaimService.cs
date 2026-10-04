using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using OpsFlow.Data;
using OpsFlow.Models.DTOs.Claims;
using OpsFlow.Models.Entities;
using OpsFlow.Services.Interfaces.Claims;
using OpsFlow.Services.Interfaces.Notifications;
using Supabase;

namespace OpsFlow.Services.Claims;

public class ClaimService : IClaimService
{
    private readonly OpsFlowDbContext _dbContext;
    private readonly Supabase.Client _supabaseClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEmailService _emailService;

    public ClaimService(
        OpsFlowDbContext dbContext,
        Supabase.Client supabaseClient,
        IHttpContextAccessor httpContextAccessor,
        IEmailService emailService)
    {
        _dbContext = dbContext;
        _supabaseClient = supabaseClient;
        _httpContextAccessor = httpContextAccessor;
        _emailService = emailService;
    }

    public async Task<ClaimResponse> CreateClaimAsync(
        CreateClaimRequest request)
    {
        var employee = await GetCurrentEmployeeAsync();

        var manager = await EnsureManager(
            employee.OrganizationId);

        var claim = new OpsFlow.Models.Entities.Claim
        {
            Title = request.Title,
            Description = request.Description,

            RaisedByUserId = employee.UserId,
            RaisedByName = employee.Name,
            RaisedByEmail = employee.Email,

            OrganizationId = employee.OrganizationId,
            OrganizationName = employee.OrganizationName,
            OrganizationLocation = employee.Location,

            ManagerUserId = manager?.UserId,
            ManagerName = manager?.Name,
            ManagerEmail = manager?.Email,

            Status = "Raised",

            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _dbContext.Claims.Add(claim);

        await _dbContext.SaveChangesAsync();

        // Photo
        if (request.Photo != null)
        {
            await AddAttachmentAsync(
                claim.Id,
                request.Photo);
        }

        // Video
        if (request.Video != null)
        {
            await AddAttachmentAsync(
                claim.Id,
                request.Video);
        }

        // History
        await AddHistoryAsync(
            claim,
            "Created",
            null,
            "Raised",
            employee.UserId,
            employee.Name,
            employee.Email,
            "Claim created");

        await _dbContext.SaveChangesAsync();

        // Manager email
        if (manager != null &&
            !string.IsNullOrWhiteSpace(manager.Email))
        {
            await SendClaimRaisedEmailAsync(
                claim,
                manager.Email);
        }

        return await BuildClaimResponseAsync(claim.Id);
    }

    public async Task<List<ClaimResponse>> GetAllClaimsAsync()
    {
        var employee = await GetCurrentEmployeeAsync();

        var claims = await _dbContext.Claims
            .Where(x =>
                x.OrganizationId == employee.OrganizationId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Id)
            .ToListAsync();

        var result = new List<ClaimResponse>();

        foreach (var id in claims)
        {
            result.Add(
                await BuildClaimResponseAsync(id));
        }

        return result;
    }



    public async Task<List<ClaimResponse>> GetMyClaimsAsync()
    {
        var employee = await GetCurrentEmployeeAsync();

        var claims = await _dbContext.Claims
            .Where(x =>
                x.RaisedByUserId == employee.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Id)
            .ToListAsync();

        var result = new List<ClaimResponse>();

        foreach (var id in claims)
        {
            result.Add(
                await BuildClaimResponseAsync(id));
        }

        return result;
    }


    public async Task<List<ClaimResponse>> GetAssignedClaimsAsync()
    {
        var employee = await GetCurrentEmployeeAsync();

        var claims = await _dbContext.Claims
            .Where(x =>
                x.AssignedEmployeeId == employee.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Id)
            .ToListAsync();

        var result = new List<ClaimResponse>();

        foreach (var id in claims)
        {
            result.Add(
                await BuildClaimResponseAsync(id));
        }

        return result;
    }

    public async Task<ClaimResponse> GetClaimByIdAsync(
        long id)
    {
        var employee = await GetCurrentEmployeeAsync();

        var claim = await _dbContext.Claims
            .FirstOrDefaultAsync(x => x.Id == id);

        if (claim == null)
            throw new Exception("Claim not found.");

        var canAccess =
            claim.RaisedByUserId == employee.UserId ||
            claim.AssignedEmployeeId == employee.UserId ||
            claim.ManagerUserId == employee.UserId;

        if (!canAccess)
            throw new UnauthorizedAccessException(
                "You do not have access to this claim.");

        return await BuildClaimResponseAsync(id);
    }

    public async Task<bool> AssignClaimAsync(
        long claimId,
        AssignClaimRequest request)
    {
        var manager = await GetCurrentEmployeeAsync();

        if (manager.Role != "Manager")
            throw new UnauthorizedAccessException(
                "Only managers can assign claims.");

        var claim = await _dbContext.Claims
            .FirstOrDefaultAsync(x => x.Id == claimId);

        if (claim == null)
            throw new Exception("Claim not found.");

        if (claim.Status != "Raised")
            throw new Exception(
                "Only Raised claims can be assigned.");

        var employee = await GetEmployeeByIdAsync(
            request.EmployeeUserId);

        if (employee == null)
            throw new Exception(
                "Employee not found.");

        if (employee.Role != "Employee")
            throw new Exception(
                "Claim can only be assigned to an Employee.");

        var oldStatus = claim.Status;

        claim.AssignedEmployeeId = employee.UserId;
        claim.AssignedEmployeeName = employee.Name;
        claim.AssignedEmployeeEmail = employee.Email;

        claim.Status = "InProgress";

        claim.AssignedAt = DateTime.UtcNow;
        claim.UpdatedAt = DateTime.UtcNow;

        await AddHistoryAsync(
            claim,
            "Assigned",
            oldStatus,
            "InProgress",
            manager.UserId,
            manager.Name,
            manager.Email,
            $"Claim assigned to {employee.Name}");

        await _dbContext.SaveChangesAsync();

        await SendClaimAssignedEmailAsync(claim);

        return true;
    }


    public async Task<bool> UpdateClaimStatusAsync(
        long claimId,
        UpdateClaimStatusRequest request)
    {
        var employee = await GetCurrentEmployeeAsync();

        if (employee.Role != "Employee")
            throw new UnauthorizedAccessException(
                "Only employees can update claim status.");

        var claim = await _dbContext.Claims
            .FirstOrDefaultAsync(x => x.Id == claimId);

        if (claim == null)
            throw new Exception("Claim not found.");

        if (claim.AssignedEmployeeId != employee.UserId)
            throw new UnauthorizedAccessException(
                "Only the assigned employee can update this claim.");

        if (claim.Status != "InProgress")
            throw new Exception(
                "Claim can only be moved to Review from InProgress.");

        if (request.Status != "Review")
            throw new Exception(
                "Employee can only move the claim to Review.");

        var oldStatus = claim.Status;

        claim.Status = "Review";

        claim.UpdatedAt = DateTime.UtcNow;

        await AddHistoryAsync(
            claim,
            "SubmittedForReview",
            oldStatus,
            "Review",
            employee.UserId,
            employee.Name,
            employee.Email,
            request.Comment);

        await _dbContext.SaveChangesAsync();

        // Manager email
        await SendClaimReviewEmailAsync(claim);

        return true;
    }

    public async Task<bool> VerifyClaimAsync(
        long claimId,
        VerifyClaimRequest request)
    {
        var manager = await GetCurrentEmployeeAsync();

        if (manager.Role != "Manager")
            throw new UnauthorizedAccessException(
                "Only managers can verify claims.");

        var claim = await _dbContext.Claims
            .FirstOrDefaultAsync(x => x.Id == claimId);

        if (claim == null)
            throw new Exception("Claim not found.");

        if (claim.Status != "Review")
            throw new Exception(
                "Only claims in Review can be verified.");

        if (request.Approved)
        {
            var oldStatus = claim.Status;

            claim.Status = "Closed";

            claim.ResolutionComment =
                request.Comment;

            claim.ResolvedAt =
                DateTime.UtcNow;

            claim.UpdatedAt =
                DateTime.UtcNow;

            await AddHistoryAsync(
                claim,
                "Approved",
                oldStatus,
                "Closed",
                manager.UserId,
                manager.Name,
                manager.Email,
                request.Comment);

            await _dbContext.SaveChangesAsync();

            await SendClaimClosedEmailAsync(claim);

            return true;
        }

        var rejectedOldStatus = claim.Status;

        claim.Status = "InProgress";

        claim.ResolutionComment =
            request.Comment;

        claim.UpdatedAt =
            DateTime.UtcNow;

        await AddHistoryAsync(
            claim,
            "Rejected",
            rejectedOldStatus,
            "InProgress",
            manager.UserId,
            manager.Name,
            manager.Email,
            request.Comment);

        await _dbContext.SaveChangesAsync();

        // Send mail to assigned employee
        await SendClaimRejectedEmailAsync(claim);

        return true;
    }


    public async Task<ClaimAttachment> GetAttachmentAsync(
        long claimId,
        long attachmentId)
    {
        var attachment = await _dbContext.ClaimAttachments
            .FirstOrDefaultAsync(x =>
                x.Id == attachmentId &&
                x.ClaimId == claimId);

        if (attachment == null)
            throw new Exception(
                "Attachment not found.");

        return attachment;
    }

    public async Task<ClaimDashboardResponse> GetDashboardAsync()
    {
        var claims = await _dbContext.Claims
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        var today = DateTime.UtcNow.Date;

        var response = new ClaimDashboardResponse
        {
            TotalIssues = claims.Count,

            RaisedIssues = claims.Count(x =>
                x.Status == "Raised"),

            InProgressIssues = claims.Count(x =>
                x.Status == "InProgress"),

            ReviewIssues = claims.Count(x =>
                x.Status == "Review"),

            ClosedIssues = claims.Count(x =>
                x.Status == "Closed"),

            TodayIssues = claims.Count(x =>
                x.CreatedAt.Date == today),

            RecentIssues = claims
                .Take(10)
                .Select(x => new DashboardClaimItem
                {
                    Id = x.Id,

                    Title = x.Title,

                    Status = x.Status,

                    RaisedByName =
                        x.RaisedByName,

                    AssignedEmployeeName =
                        x.AssignedEmployeeName,

                    OrganizationName =
                        x.OrganizationName,

                    CreatedAt =
                        x.CreatedAt,

                    UpdatedAt =
                        x.UpdatedAt
                })
                .ToList()
        };

        return response;
    }

    private async Task<Employee> GetCurrentEmployeeAsync()
    {
        var user =
            _httpContextAccessor
                .HttpContext?
                .User;

        if (user == null ||
            user.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException(
                "User is not authenticated.");
        }

        var userId =
            user.FindFirst(
                ClaimTypes.NameIdentifier)?.Value
            ??
            user.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "User ID not found in token.");
        }

        var response =
            await _supabaseClient
                .From<Employee>()
                .Where(x => x.UserId == userId)
                .Get();

        var employee =
            response.Models.FirstOrDefault();

        if (employee == null)
            throw new UnauthorizedAccessException(
                "Employee profile not found.");

        return employee;
    }

    private async Task<Employee?> GetEmployeeByIdAsync(
        string userId)
    {
        var response =
            await _supabaseClient
                .From<Employee>()
                .Where(x => x.UserId == userId)
                .Get();

        return response.Models.FirstOrDefault();
    }

    private async Task<Employee?> EnsureManager(
        Guid organizationId)
    {
        var response =
            await _supabaseClient
                .From<Employee>()
                .Where(x =>
                    x.OrganizationId == organizationId &&
                    x.Role == "Manager")
                .Get();

        return response.Models.FirstOrDefault();
    }


    private async Task AddHistoryAsync(
        OpsFlow.Models.Entities.Claim claim,
        string action,
        string? fromStatus,
        string? toStatus,
        string userId,
        string name,
        string email,
        string? comment)
    {
        var history = new ClaimHistory
        {
            ClaimId = claim.Id,

            Action = action,

            FromStatus = fromStatus,

            ToStatus = toStatus,

            PerformedByUserId = userId,

            PerformedByName = name,

            PerformedByEmail = email,

            Comment = comment,

            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ClaimHistory.Add(history);
    }

    private async Task AddAttachmentAsync(
        long claimId,
        IFormFile file)
    {
        using var memoryStream =
            new MemoryStream();

        await file.CopyToAsync(memoryStream);

        var attachment = new ClaimAttachment
        {
            ClaimId = claimId,

            FileName = file.FileName,

            ContentType = file.ContentType,

            FileData = memoryStream.ToArray(),

            CreatedAt = DateTime.UtcNow
        };

        _dbContext.ClaimAttachments.Add(
            attachment);

        await _dbContext.SaveChangesAsync();
    }

    private async Task<ClaimResponse> BuildClaimResponseAsync(
        long claimId)
    {
        var claim = await _dbContext.Claims
            .Include(x => x.Attachments)
            .Include(x => x.History)
            .FirstOrDefaultAsync(x => x.Id == claimId);

        if (claim == null)
            throw new Exception("Claim not found.");

        return new ClaimResponse
        {
            Id = claim.Id,

            Title = claim.Title,

            Description = claim.Description,

            RaisedByUserId =
                claim.RaisedByUserId,

            RaisedByName =
                claim.RaisedByName,

            RaisedByEmail =
                claim.RaisedByEmail,

            OrganizationId =
                claim.OrganizationId,

            OrganizationName =
                claim.OrganizationName,

            OrganizationLocation =
                claim.OrganizationLocation,

            ManagerUserId =
                claim.ManagerUserId,

            ManagerName =
                claim.ManagerName,

            ManagerEmail =
                claim.ManagerEmail,

            AssignedEmployeeId =
                claim.AssignedEmployeeId,

            AssignedEmployeeName =
                claim.AssignedEmployeeName,

            AssignedEmployeeEmail =
                claim.AssignedEmployeeEmail,

            Status =
                claim.Status,

            ResolutionComment =
                claim.ResolutionComment,

            CreatedAt =
                claim.CreatedAt,

            AssignedAt =
                claim.AssignedAt,

            UpdatedAt =
                claim.UpdatedAt,

            ResolvedAt =
                claim.ResolvedAt,

            Attachments =
                claim.Attachments
                    .Select(x => new ClaimAttachmentResponse
                    {
                        Id = x.Id,

                        FileName =
                            x.FileName,

                        ContentType =
                            x.ContentType,

                        CreatedAt =
                            x.CreatedAt
                    })
                    .ToList(),

            History =
                claim.History
                    .OrderBy(x => x.CreatedAt)
                    .Select(x =>
                        new ClaimHistoryResponse
                        {
                            Id = x.Id,

                            Action =
                                x.Action,

                            FromStatus =
                                x.FromStatus,

                            ToStatus =
                                x.ToStatus,

                            PerformedByUserId =
                                x.PerformedByUserId,

                            PerformedByName =
                                x.PerformedByName,

                            PerformedByEmail =
                                x.PerformedByEmail,

                            Comment =
                                x.Comment,

                            CreatedAt =
                                x.CreatedAt
                        })
                    .ToList()
        };
    }

    // EMAIL - CLAIM RAISED

    private async Task SendClaimRaisedEmailAsync(
        OpsFlow.Models.Entities.Claim claim,
        string managerEmail)
    {
        var subject =
            $"New Claim Raised - #{claim.Id}";

        var body = $"""
            <h2>New Claim Raised</h2>

            <p>A new issue has been raised.</p>

            <p><b>Claim ID:</b> {claim.Id}</p>

            <p><b>Title:</b> {claim.Title}</p>

            <p><b>Description:</b> {claim.Description}</p>

            <p><b>Raised By:</b> {claim.RaisedByName}</p>

            <p><b>Organization:</b> {claim.OrganizationName}</p>

            <p><b>Status:</b> Raised</p>
            """;

        await _emailService.SendEmailAsync(
            managerEmail,
            subject,
            body);
    }

    // EMAIL - CLAIM ASSIGNED

    private async Task SendClaimAssignedEmailAsync(
        OpsFlow.Models.Entities.Claim claim)
    {
        if (string.IsNullOrWhiteSpace(
            claim.AssignedEmployeeEmail))
        {
            return;
        }

        var subject =
            $"Claim Assigned - #{claim.Id}";

        var body = $"""
            <h2>Claim Assigned</h2>

            <p>A claim has been assigned to you.</p>

            <p><b>Claim ID:</b> {claim.Id}</p>

            <p><b>Title:</b> {claim.Title}</p>

            <p><b>Description:</b> {claim.Description}</p>

            <p><b>Raised By:</b> {claim.RaisedByName}</p>

            <p><b>Organization:</b> {claim.OrganizationName}</p>

            <p><b>Status:</b> In Progress</p>

            <p>Please work on the issue and submit it for review.</p>
            """;

        await _emailService.SendEmailAsync(
            claim.AssignedEmployeeEmail,
            subject,
            body);
    }

    // EMAIL - REVIEW

    private async Task SendClaimReviewEmailAsync(
        OpsFlow.Models.Entities.Claim claim)
    {
        if (string.IsNullOrWhiteSpace(
            claim.ManagerEmail))
        {
            return;
        }

        var subject =
            $"Claim Ready for Review - #{claim.Id}";

        var body = $"""
            <h2>Claim Ready for Review</h2>

            <p>The assigned employee has completed the work.</p>

            <p><b>Claim ID:</b> {claim.Id}</p>

            <p><b>Title:</b> {claim.Title}</p>

            <p><b>Raised By:</b> {claim.RaisedByName}</p>

            <p><b>Assigned Employee:</b> {claim.AssignedEmployeeName}</p>

            <p><b>Status:</b> Review</p>

            <p>Please review and approve or reject the claim.</p>
            """;

        await _emailService.SendEmailAsync(
            claim.ManagerEmail,
            subject,
            body);
    }

    // EMAIL - CLAIM CLOSED

    private async Task SendClaimClosedEmailAsync(
        OpsFlow.Models.Entities.Claim claim)
    {
        if (string.IsNullOrWhiteSpace(
            claim.RaisedByEmail))
        {
            return;
        }

        var subject =
            $"Claim Closed - #{claim.Id}";

        var body = $"""
            <h2>Claim Closed</h2>

            <p>Your issue has been successfully reviewed and closed.</p>

            <p><b>Claim ID:</b> {claim.Id}</p>

            <p><b>Title:</b> {claim.Title}</p>

            <p><b>Status:</b> Closed</p>

            <p><b>Resolution:</b> {claim.ResolutionComment}</p>

            <p>Thank you for using OpsFlow.</p>
            """;

        await _emailService.SendEmailAsync(
            claim.RaisedByEmail,
            subject,
            body);
    }

    // EMAIL - CLAIM REJECTED

    private async Task SendClaimRejectedEmailAsync(
        OpsFlow.Models.Entities.Claim claim)
    {
        if (string.IsNullOrWhiteSpace(
            claim.AssignedEmployeeEmail))
        {
            return;
        }

        var subject =
            $"Claim Rejected - Action Required - #{claim.Id}";

        var body = $"""
            <h2>Claim Rejected</h2>

            <p>The manager has rejected the claim during review.</p>

            <p><b>Claim ID:</b> {claim.Id}</p>

            <p><b>Title:</b> {claim.Title}</p>

            <p><b>Status:</b> In Progress</p>

            <p><b>Manager Comment:</b> {claim.ResolutionComment}</p>

            <p>Please work on the issue again and submit it for review.</p>
            """;

        await _emailService.SendEmailAsync(
            claim.AssignedEmployeeEmail,
            subject,
            body);
    }
}