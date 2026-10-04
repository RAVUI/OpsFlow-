namespace OpsFlow.Models.DTOs.Auth;

public class UpdateRoleRequest
{
    public string UserId { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}