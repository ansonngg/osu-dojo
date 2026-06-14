using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace OsuDojo.Infrastructure.Model;

[Table("rank_certificate")]
public class RankCertificate : BaseModel
{
    [PrimaryKey("id")]
    public int Id { get; init; }

    [Column("user_id")]
    public int UserId { get; init; }

    [Column("exam_session_id")]
    public int ExamSessionId { get; init; }

    [Column("pass_grade")]
    public int PassGrade { get; init; }

    [Column("detailed_result")]
    public string DetailedResult { get; init; } = string.Empty;
}
