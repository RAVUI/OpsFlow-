namespace OpsFlow.Models.DTOs.Auth;

public class OrganizationResponse
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}