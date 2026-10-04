namespace OpsFlow.Models.DTOs.Claims;

public class ClaimResponse
{
    public long Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string RaisedByUserId { get; set; } = string.Empty;

    public string RaisedByName { get; set; } = string.Empty;

    public string RaisedByEmail { get; set; } = string.Empty;

    public Guid OrganizationId { get; set; }

    public string OrganizationName { get; set; } = string.Empty;

    public string OrganizationLocation { get; set; } = string.Empty;

    public string? ManagerUserId { get; set; }

    public string? ManagerName { get; set; }

    public string? ManagerEmail { get; set; }

    public string? AssignedEmployeeId { get; set; }

    public string? AssignedEmployeeName { get; set; }

    public string? AssignedEmployeeEmail { get; set; }

    public string Status { get; set; } = string.Empty;

    public string? ResolutionComment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public List<ClaimAttachmentResponse> Attachments { get; set; }
        = new();

    public List<ClaimHistoryResponse> History { get; set; }
        = new();
}


public class ClaimAttachmentResponse
{
    public long Id { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}


public class ClaimHistoryResponse
{
    public long Id { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? FromStatus { get; set; }

    public string? ToStatus { get; set; }

    public string PerformedByUserId { get; set; } = string.Empty;

    public string PerformedByName { get; set; } = string.Empty;

    public string PerformedByEmail { get; set; } = string.Empty;

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }
}