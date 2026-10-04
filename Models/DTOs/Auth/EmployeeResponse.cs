namespace OpsFlow.Models.DTOs.Auth;

public class EmployeeResponse
{
    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public Guid OrganizationId { get; set; }

    public string OrganizationName { get; set; } = string.Empty;

    public string Location { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}