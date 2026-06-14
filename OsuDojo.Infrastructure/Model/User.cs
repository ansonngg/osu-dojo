using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace OsuDojo.Infrastructure.Model;

[Table("user")]
public class User : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; init; }

    [Column("osu_id")]
    public int OsuId { get; init; }

    [Column("roles")]
    public string[] Roles { get; init; } = ["user"];

    [Column("osu_rank")]
    public int OsuRank { get; init; }

    [Column("taiko_rank")]
    public int TaikoRank { get; init; }

    [Column("catch_rank")]
    public int CatchRank { get; init; }

    [Column("mania_rank")]
    public int ManiaRank { get; init; }
}
