using System.ComponentModel.DataAnnotations;

namespace OpsFlow.Models.Entities;

public class Claim
{
    public long Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string RaisedByUserId { get; set; } = string.Empty;

    [Required]
    public string RaisedByName { get; set; } = string.Empty;

    [Required]
    public string RaisedByEmail { get; set; } = string.Empty;

    public Guid OrganizationId { get; set; }

    [Required]
    public string OrganizationName { get; set; } = string.Empty;

    [Required]
    public string OrganizationLocation { get; set; } = string.Empty;

    public string? ManagerUserId { get; set; }

    public string? ManagerName { get; set; }

    public string? ManagerEmail { get; set; }

    public string? AssignedEmployeeId { get; set; }

    public string? AssignedEmployeeName { get; set; }

    public string? AssignedEmployeeEmail { get; set; }

    [Required]
    public string Status { get; set; } = "Raised";

    public string? ResolutionComment { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? AssignedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public ICollection<ClaimAttachment> Attachments { get; set; }
        = new List<ClaimAttachment>();

    public ICollection<ClaimHistory> History { get; set; }
        = new List<ClaimHistory>();
}