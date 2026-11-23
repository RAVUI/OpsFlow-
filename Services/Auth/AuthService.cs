using OpsFlow.Models.DTOs.Auth;
using OpsFlow.Models.Entities;
using OpsFlow.Services.Interfaces.Auth;
using Supabase;

namespace OpsFlow.Services.Auth;

public class AuthService : IAuthService
{
    private readonly Client _supabase;

    public AuthService(Client supabase)
    {
        _supabase = supabase;
    }

    public async Task<AuthResponse> RegisterAsync(
    RegisterRequest request)
    {
        // Get organization
        var organizationResponse = await _supabase
            .From<Organization>()
            .Where(x => x.Id == request.OrganizationId)
            .Get();

        var organization =
            organizationResponse.Models.FirstOrDefault();

        if (organization == null)
        {
            throw new Exception("Organization not found.");
        }

        // Register user in Supabase Auth
        var session = await _supabase.Auth.SignUp(
            request.Email,
            request.Password);

        if (session?.User == null)
        {
            throw new Exception(
                "User registration failed.");
        }

        var userId = session.User.Id;

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new Exception(
                "Supabase user ID was not returned.");
        }

        // Create employee profile
        var employee = new Employee
        {
            UserId = userId,

            Name = request.Name,

            Email = request.Email,

            Role = "User",

            OrganizationId = organization.Id,

            OrganizationName = organization.Name,

            Location = organization.Location,

            CreatedAt = DateTime.UtcNow
        };

        await _supabase
            .From<Employee>()
            .Insert(employee);

        return new AuthResponse
        {
            AccessToken =
                session.AccessToken ?? string.Empty,

            RefreshToken =
                session.RefreshToken ?? string.Empty,

            ExpiresIn =
                session.ExpiresIn,

            Employee = new EmployeeInfo
            {
                UserId = employee.UserId,

                Name = employee.Name,

                Email = employee.Email,

                Role = employee.Role,

                OrganizationId =
                    employee.OrganizationId
            }
        };
    }

    public async Task<AuthResponse> LoginAsync(
        LoginRequest request)
    {
        // Login through Supabase
        var session = await _supabase.Auth.SignIn(
            request.Email,
            request.Password ,request.Number);

        if (session?.User == null)
        {
            throw new Exception(
                "Invalid email or password.");
        }

        // Get employee profile
        var employeeResponse = await _supabase
            .From<Employee>()
            .Where(x =>
                x.UserId == session.User.Id)
            .Get();

        var employee =
            employeeResponse.Models.FirstOrDefault();

        if (employee == null)
        {
            throw new Exception(
                "Employee profile not found.");
        }

        return new AuthResponse
        {
            AccessToken =
                session.AccessToken ?? string.Empty,

            RefreshToken =
                session.RefreshToken ?? string.Empty,

            ExpiresIn =
                session.ExpiresIn,

            Employee = new EmployeeInfo
            {
                UserId = employee.UserId,
                Name = employee.Name,
                Email = employee.Email,
                Role = employee.Role,
                OrganizationId =
                    employee.OrganizationId
            }
        };
    }

    public async Task<bool> UpdateRoleAsync(
        UpdateRoleRequest request)
    {
        if (request.Role != "Employee" &&
            request.Role != "Manager" &&
            request.Role != "User")
        {
            throw new Exception(
                "Invalid role.");
        }

        var response = await _supabase
            .From<Employee>()
            .Where(x =>
                x.UserId == request.UserId)
            .Get();

        var employee =
            response.Models.FirstOrDefault();

        if (employee == null)
        {
            throw new Exception(
                "Employee not found.");
        }

        employee.Role = request.Role;

        await employee.Update<Employee>();

        return true;
    }

    public async Task<List<EmployeeResponse>>
        GetUsersAsync()
    {
        var response = await _supabase
            .From<Employee>()
            .Get();

        return response.Models
            .Select(x => new EmployeeResponse
            {
                UserId = x.UserId,
                Name = x.Name,
                Email = x.Email,
                Role = x.Role,
                OrganizationId =
                    x.OrganizationId,
                OrganizationName = x.OrganizationName,
                Location = x.Location,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }

    public async Task<List<OrganizationResponse>>
        GetOrganizationsAsync()
    {
        var response = await _supabase
            .From<Organization>()
            .Get();

        return response.Models
            .Select(x => new OrganizationResponse
            {
                Id = x.Id,
                Name = x.Name,
                Location = x.Location,
                CreatedAt = x.CreatedAt
            })
            .ToList();
    }

    public async Task<UserProfileResponse> GetCurrentUserAsync(
    string userId)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new UnauthorizedAccessException(
                "User ID is missing.");
        }

        var employee = await _supabase
            .From<Employee>()
            .Where(x => x.UserId == userId)
            .Single();

        if (employee == null)
        {
            throw new KeyNotFoundException(
                "User profile not found.");
        }

        return new UserProfileResponse
        {
            UserId = employee.UserId,
            Name = employee.Name,
            Email = employee.Email,
            Role = employee.Role,
            OrganizationId = employee.OrganizationId,
            OrganizationName = employee.OrganizationName,
            Location = employee.Location
        };
    }
}