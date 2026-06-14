using OsuDojo.Application.Exam;

namespace OsuDojo.Application.Context;

public class ExamSessionContext
{
    public int ExamSessionId { get; init; }
    public int UserId { get; init; }
    public int OsuId { get; init; }
    public required string GameMode { get; init; }
    public int Rank { get; init; }
    public int RoomId { get; init; }
    public required IExamTracker ExamTracker { get; init; }

    public ExamSessionStatus Status
    {
        get;
        set
        {
            field = value;
            LastUpdatedAt = DateTime.UtcNow;
        }
    } = ExamSessionStatus.Waiting;

    public DateTime LastUpdatedAt { get; private set; } = DateTime.UtcNow;
}
