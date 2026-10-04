using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpsFlow.Models.DTOs.Auth;
using OpsFlow.Services.Interfaces.Auth;
using System.Security.Claims;

namespace OpsFlow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        try
        {
            var result =
                await _authService.RegisterAsync(request);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        try
        {
            var result =
                await _authService.LoginAsync(request);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Unauthorized(new
            {
                message = ex.Message
            });
        }
    }


    [Authorize]
    [HttpPut("users/role")]
    public async Task<IActionResult> UpdateRole(
        [FromBody] UpdateRoleRequest request)
    {
        try
        {
            await _authService.UpdateRoleAsync(request);

            return Ok(new
            {
                message =
                    "User role updated successfully."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    [Authorize]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users =
                await _authService.GetUsersAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }


    [HttpGet("organizations")]
    public async Task<IActionResult> GetOrganizations()
    {
        try
        {
            var organizations =
                await _authService
                    .GetOrganizationsAsync();

            return Ok(organizations);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId =
            User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        try
        {
            var user =
                await _authService.GetCurrentUserAsync(userId);

            return Ok(user);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }
}