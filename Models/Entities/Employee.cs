using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace OpsFlow.Models.Entities;

[Table("employees")]
public class Employee : BaseModel
{
    [PrimaryKey("user_id", true)]
    public string UserId { get; set; } = string.Empty;

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("role")]
    public string Role { get; set; } = "User";

    [Column("organization_id")]
    public Guid OrganizationId { get; set; }

    [Column("organization_name")]
    public string OrganizationName { get; set; } = string.Empty;

    [Column("location")]
    public string Location { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}