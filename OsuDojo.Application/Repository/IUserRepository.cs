using OsuDojo.Application.Query;

namespace OsuDojo.Application.Repository;

public interface IUserRepository
{
    Task<UserRoleQuery?> GetRoleAsync(int osuId);
    Task<UserRankQuery> GetRankAsync(int osuId, string gameMode);
    Task<UserRoleQuery> CreateAsync(int osuId);
    Task UpdateUserBestCertificateAsync(int userId, string gameMode, int rank, int rankCertificateId);
}
