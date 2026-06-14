using OsuDojo.Application.Query;

namespace OsuDojo.Application.Repository;

public interface IUserRepository
{
    Task<UserRoleQuery> GetRoleAsync(int userId);
    Task<UserRoleQuery?> GetRoleByOsuIdAsync(int osuId);
    Task<UserRankQuery> GetRankAsync(int userId, string gameMode);
    Task<UserRoleQuery> CreateAsync(int osuId);
    Task UpdateUserBestCertificateAsync(int userId, string gameMode, int rank, int rankCertificateId);
}
