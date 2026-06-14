namespace OsuDojo.Application.Service;

public interface ISessionService
{
    Task SaveSessionAsync(string sessionId, object sessionData, TimeSpan? timeToLive = null);
    Task<T?> GetSessionAsync<T>(string sessionId);
    Task DeleteSessionAsync(string sessionId);
    Task<bool> ExistsSessionAsync(string sessionId);
}
