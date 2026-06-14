using System.Text.Json.Serialization;
using OsuDojo.Application.Exam;

namespace OsuDojo.Web.Response;

public class ExamResponse
{
    [JsonPropertyName("rank")]
    public int Rank { get; init; }

    [JsonPropertyName("exam_content")]
    public ExamContent ExamContent { get; init; } = new();
}
