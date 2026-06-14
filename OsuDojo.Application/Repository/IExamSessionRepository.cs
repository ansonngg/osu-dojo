namespace OsuDojo.Application.Repository;

public interface IExamSessionRepository
{
    public Task<int> CreateAsync(int userId, int examId);
    public Task<int?> GetInProgressIdAsync(int userId);
    public Task ProceedToNextStageAsync(int examSessionId);
    public Task SetCompletedAsync(int examSessionId);
    public Task SetTimeOutAsync(int examSessionId);
    public Task DisqualifyAsync(int examSessionId);
    public Task SetNoResponseAsync(int examSessionId);
    public Task TerminateAsync(int examSessionId);
}
