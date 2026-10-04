using OpsFlow.Models.DTOs.Claims;
using OpsFlow.Models.Entities;

namespace OpsFlow.Services.Interfaces.Claims;

public interface IClaimService
{
    Task<ClaimResponse> CreateClaimAsync(CreateClaimRequest request);

    Task<List<ClaimResponse>> GetAllClaimsAsync();

    Task<List<ClaimResponse>> GetMyClaimsAsync();

    Task<List<ClaimResponse>> GetAssignedClaimsAsync();

    Task<ClaimResponse> GetClaimByIdAsync(long id);

    Task<bool> AssignClaimAsync(
        long claimId,
        AssignClaimRequest request);

    Task<bool> UpdateClaimStatusAsync(
        long claimId,
        UpdateClaimStatusRequest request);

    Task<bool> VerifyClaimAsync(
        long claimId,
        VerifyClaimRequest request);

    Task<ClaimAttachment> GetAttachmentAsync(
        long claimId,
        long attachmentId);

    Task<ClaimDashboardResponse> GetDashboardAsync();
}