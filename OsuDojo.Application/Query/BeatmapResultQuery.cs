using OsuDojo.Application.Exam;

namespace OsuDojo.Application.Query;

public class BeatmapResultQuery
{
    public StageResult Result { get; init; } = new();
    public required string[] Mods { get; init; }
}
