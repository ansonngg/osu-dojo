using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace OsuDojo.Infrastructure.Model;

[Table("user_best_certificate")]
public class UserBestCertificate : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; init; }

    [Column("user_id")]
    public int UserId { get; init; }

    [Column("game_mode")]
    public string GameMode { get; init; } = string.Empty;

    [Column("rank")]
    public int Rank { get; init; }

    [Column("rank_certificate_id")]
    public int RankCertificateId { get; init; }
}
