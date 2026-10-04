namespace OpsFlow.Models.DTOs.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public long ExpiresIn { get; set; }

    public EmployeeInfo Employee { get; set; } = new();
}

public class EmployeeInfo
{
    public string UserId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public Guid OrganizationId { get; set; }
    public string OrganizationName { get; set;} = string.Empty;
    public string Location { get; set; }= string.Empty;
}