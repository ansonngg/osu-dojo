using OsuDojo.Application.Exam;

namespace OsuDojo.Application.Query;

public class ExamQuery
{
    public int ExamId { get; init; }
    public int Rank { get; init; }
    public ExamContent ExamContent { get; init; } = new();
    public int? RequiredRank { get; init; }
}
