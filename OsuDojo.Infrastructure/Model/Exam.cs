using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace OsuDojo.Infrastructure.Model;

[Table("exam")]
public class Exam : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; init; }

    [Column("game_mode")]
    public string GameMode { get; init; } = string.Empty;

    [Column("rank")]
    public int Rank { get; init; }

    [Column("exam_content")]
    public string ExamContent { get; init; } = string.Empty;

    [Column("required_rank")]
    public int? RequiredRank { get; init; }

    [Column("is_active")]
    public bool IsActive { get; init; } = true;
}
