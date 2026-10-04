namespace OpsFlow.Models.DTOs.Claims;

public class VerifyClaimRequest
{
    public bool Approved { get; set; }

    public string? Comment { get; set; }
}