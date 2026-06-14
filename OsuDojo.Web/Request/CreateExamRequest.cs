using System.Text.Json.Serialization;
using OsuDojo.Application.Exam;

namespace OsuDojo.Web.Request;

public class CreateExamRequest
{
    [JsonPropertyName("game_mode")]
    public string GameMode { get; init; } = string.Empty;

    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    [JsonPropertyName("exam_content")]
    public ExamContent ExamContent { get; init; } = new();

    [JsonPropertyName("required_rank")]
    public int? RequiredRank { get; init; }
}
