namespace OpsFlow.Models.Entities;

public class ClaimHistory
{
    public long Id { get; set; }

    public long ClaimId { get; set; }

    public string Action { get; set; } = string.Empty;

    public string? FromStatus { get; set; }

    public string? ToStatus { get; set; }

    public string PerformedByUserId { get; set; } = string.Empty;

    public string PerformedByName { get; set; } = string.Empty;

    public string PerformedByEmail { get; set; } = string.Empty;

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }

    public Claim Claim { get; set; } = null!;
}