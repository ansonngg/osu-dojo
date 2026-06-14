using OsuDojo.Application.Query;
using OsuDojo.Application.Repository;
using OsuDojo.Infrastructure.Model;
using Supabase.Postgrest;

namespace OsuDojo.Infrastructure.Repository;

public class UserRepository(Supabase.Client database) : IUserRepository
{
    private readonly Supabase.Client _database = database;

    public async Task<UserRoleQuery?> GetRoleAsync(int osuId)
    {
        var response = await _database
            .From<User>()
            .Select(x => new object[] { x.Id, x.Roles })
            .Where(x => x.OsuId == osuId)
            .Single();

        return response != null ? new UserRoleQuery { UserId = response.Id, Roles = response.Roles } : null;
    }

    public async Task<UserRankQuery> GetRankAsync(int osuId, string gameMode)
    {
        switch (gameMode)
        {
            case "osu":
            {
                var response = await _database
                    .From<User>()
                    .Select(x => new object[] { x.OsuRank })
                    .Where(x => x.OsuId == osuId)
                    .Single();

                return response != null
                    ? new UserRankQuery { Rank = response.OsuRank }
                    : throw new NullReferenceException("Returned user is null.");
            }
            case "taiko":
            {
                var response = await _database
                    .From<User>()
                    .Select(x => new object[] { x.TaikoRank })
                    .Where(x => x.OsuId == osuId)
                    .Single();

                return response != null
                    ? new UserRankQuery { Rank = response.TaikoRank }
                    : throw new NullReferenceException("Returned user is null.");
            }
            case "catch":
            {
                var response = await _database
                    .From<User>()
                    .Select(x => new object[] { x.CatchRank })
                    .Where(x => x.OsuId == osuId)
                    .Single();

                return response != null
                    ? new UserRankQuery { Rank = response.CatchRank }
                    : throw new NullReferenceException("Returned user is null.");
            }
            default:
            {
                var response = await _database
                    .From<User>()
                    .Select(x => new object[] { x.ManiaRank })
                    .Where(x => x.OsuId == osuId)
                    .Single();

                return response != null
                    ? new UserRankQuery { Rank = response.ManiaRank }
                    : throw new NullReferenceException("Returned user is null.");
            }
        }
    }

    public async Task<UserRoleQuery> CreateAsync(int osuId)
    {
        var response = await _database
            .From<User>()
            .Insert(
                new User { OsuId = osuId },
                new QueryOptions { Returning = QueryOptions.ReturnType.Representation });

        return response.Model != null
            ? new UserRoleQuery { UserId = response.Model.Id, Roles = response.Model.Roles }
            : throw new NullReferenceException("Returned user is null.");
    }

    public async Task UpdateHighestRankCertificateAsync(int userId, string gameMode, int rank, int rankCertificateId)
    {
        await _database.Rpc(
            "update_user_best_certificate",
            new
            {
                p_user_id = userId,
                p_game_mode = gameMode,
                p_rank = rank,
                p_rank_certificate_id = rankCertificateId
            });
    }
}
