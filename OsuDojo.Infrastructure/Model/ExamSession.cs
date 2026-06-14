using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace OsuDojo.Infrastructure.Model;

[Table("exam_session")]
public class ExamSession : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; init; }

    [Column("user_id")]
    public int UserId { get; init; }

    [Column("exam_id")]
    public int ExamId { get; init; }

    [Column("last_reached_stage")]
    public int LastReachedStage { get; init; } = 1;

    [Column("started_at")]
    public DateTime StartedAt { get; init; } = DateTime.UtcNow;

    [Column("ended_at")]
    public DateTime? EndedAt { get; init; }

    [Column("status")]
    public string Status { get; init; } = "in_progress";
}
