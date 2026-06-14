using System.Text.Json;
using System.Text.Json.Serialization;

namespace OsuDojo.Application.Exam;

public class ExamContent
{
    [JsonPropertyName("beatmap_contents")]
    public BeatmapContent[] BeatmapContents { get; init; } = [];

    [JsonPropertyName("criteria_tables")]
    public JsonElement[] CriteriaTables { get; init; } = [];
}
