namespace OpsFlow.Models.Entities;

public class ClaimAttachment
{
    public long Id { get; set; }

    public long ClaimId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string ContentType { get; set; } = string.Empty;

    public byte[] FileData { get; set; } = Array.Empty<byte>();

    public DateTime CreatedAt { get; set; }
    public Claim Claim { get; set; } = null!;
}