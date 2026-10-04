using OpsFlow.Models.DTOs.Auth;

namespace OpsFlow.Services.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(
        RegisterRequest request);

    Task<AuthResponse> LoginAsync(
        LoginRequest request);

    Task<bool> UpdateRoleAsync(
        UpdateRoleRequest request);

    Task<List<EmployeeResponse>> GetUsersAsync();

    Task<List<OrganizationResponse>> GetOrganizationsAsync();
    Task<UserProfileResponse> GetCurrentUserAsync(string userId);

}