using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Models.DTOs.Claims;
using OpsFlow.Services.Interfaces.Claims;

namespace OpsFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClaimsController : ControllerBase
{
    private readonly IClaimService _claimService;

    public ClaimsController(IClaimService claimService)
    {
        _claimService = claimService;
    }


    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateClaim(
        [FromForm] CreateClaimRequest request)
    {
        try
        {
            var result =
                await _claimService.CreateClaimAsync(request);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClaims()
    {
        try
        {
            var result =
                await _claimService.GetAllClaimsAsync();

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // GET MY CLAIMS
    // USER
    // =========================================================

    [HttpGet("my")]
    public async Task<IActionResult> GetMyClaims()
    {
        try
        {
            var result =
                await _claimService.GetMyClaimsAsync();

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // GET ASSIGNED CLAIMS
    // EMPLOYEE
    // =========================================================

    [HttpGet("assigned")]
    public async Task<IActionResult> GetAssignedClaims()
    {
        try
        {
            var result =
                await _claimService.GetAssignedClaimsAsync();

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // GET CLAIM BY ID
    // =========================================================

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetClaim(
        long id)
    {
        try
        {
            var result =
                await _claimService.GetClaimByIdAsync(id);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // ASSIGN CLAIM
    // MANAGER
    // =========================================================

    [HttpPut("{id:long}/assign")]
    public async Task<IActionResult> AssignClaim(
        long id,
        AssignClaimRequest request)
    {
        try
        {
            await _claimService.AssignClaimAsync(
                id,
                request);

            return Ok(new
            {
                message =
                    "Claim assigned successfully."
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // UPDATE STATUS
    // EMPLOYEE
    // =========================================================

    [HttpPut("{id:long}/status")]
    public async Task<IActionResult> UpdateStatus(
        long id,
        UpdateClaimStatusRequest request)
    {
        try
        {
            await _claimService.UpdateClaimStatusAsync(
                id,
                request);

            return Ok(new
            {
                message =
                    "Claim status updated successfully."
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // VERIFY CLAIM
    // MANAGER
    // =========================================================

    [HttpPut("{id:long}/verify")]
    public async Task<IActionResult> VerifyClaim(
        long id,
        VerifyClaimRequest request)
    {
        try
        {
            await _claimService.VerifyClaimAsync(
                id,
                request);

            return Ok(new
            {
                message =
                    request.Approved
                        ? "Claim verified and closed successfully."
                        : "Claim rejected and moved back to InProgress."
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    // =========================================================
    // DOWNLOAD ATTACHMENT
    // =========================================================

    [HttpGet("{claimId:long}/attachments/{attachmentId:long}")]
    public async Task<IActionResult> GetAttachment(
     long claimId,
     long attachmentId)
    {
        try
        {
            var attachment =
                await _claimService.GetAttachmentAsync(
                    claimId,
                    attachmentId);

            return File(
                attachment.FileData,
                attachment.ContentType,
                attachment.FileName);
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(403, new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [Authorize]
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard()
    {
        try
        {
            var dashboard =
                await _claimService.GetDashboardAsync();

            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = ex.Message
            });
        }
    }
}