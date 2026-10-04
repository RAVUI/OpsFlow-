using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace OpsFlow.Models.Entities;

[Table("organizations")]
public class Organization : BaseModel
{
    [PrimaryKey("id", false)]
    public Guid Id { get; set; }

    [Column("name")]
    public string Name { get; set; } = string.Empty;

    [Column("location")]
    public string Location { get; set; } = string.Empty;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
}