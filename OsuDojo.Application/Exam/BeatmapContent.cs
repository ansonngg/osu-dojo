using System.Text.Json;
using System.Text.Json.Serialization;

namespace OsuDojo.Application.Exam;

public class BeatmapContent
{
    [JsonPropertyName("beatmap_id")]
    public int BeatmapId { get; init; }

    [JsonPropertyName("criteria_tables")]
    public JsonElement[] CriteriaTables { get; init; } = [];
}
