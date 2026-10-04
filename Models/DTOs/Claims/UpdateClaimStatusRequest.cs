namespace OpsFlow.Models.DTOs.Claims;

public class UpdateClaimStatusRequest
{
    public string Status { get; set; } = string.Empty;

    public string? Comment { get; set; }
}