using System.Text.Json.Serialization;

namespace OsuDojo.Application.Context;

public class LoginSessionContext
{
    [JsonPropertyName("user_id")]
    public int UserId { get; init; }

    [JsonPropertyName("osu_id")]
    public int OsuId { get; init; }

    [JsonPropertyName("roles")]
    public string[] Roles { get; init; } = [];

    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = string.Empty;

    [JsonPropertyName("expires_at")]
    public DateTime ExpiresAt { get; init; }
}
