using System.Text.Json;
using System.Text.Json.Serialization;

namespace OsuDojo.Application.Exam;

public class ExamContent
{
    [JsonPropertyName("beatmap_contents")]
    public BeatmapContent[] BeatmapContents { get; init; } = [];

    [JsonPropertyName("overall_grade_cutoffs")]
    public JsonElement[] OverallGradeCutoffs { get; init; } = [];
}
