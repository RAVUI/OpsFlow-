using Microsoft.AspNetCore.Http;

namespace OpsFlow.Models.DTOs.Claims;

public class CreateClaimRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public IFormFile? Photo { get; set; }

    public IFormFile? Video { get; set; }
}