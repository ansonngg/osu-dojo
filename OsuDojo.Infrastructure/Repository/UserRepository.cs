using OsuDojo.Application.Query;
using OsuDojo.Application.Repository;
using OsuDojo.Infrastructure.Model;
using Supabase.Postgrest;

namespace OsuDojo.Infrastructure.Repository;

public class UserRepository(Supabase.Client database) : IUserRepository
{
    private readonly Supabase.Client _database = database;

    public async Task<UserRoleQuery> GetRoleAsync(int userId)
    {
        var response = await _database
            .From<User>()
            .Select(x => new object[] { x.Roles })
            .Where(x => x.Id == userId)
            .Single();

        return response != null
            ? new UserRoleQuery { UserId = userId, Roles = response.Roles }
            : throw new NullReferenceException("Returned user is null.");
    }

    public async Task<UserRoleQuery?> GetRoleByOsuIdAsync(int osuId)
    {
        var response = await _database
            .From<User>()
            .Select(x => new object[] { x.Id, x.Roles })
            .Where(x => x.OsuId == osuId)
            .Single();

        return response != null ? new UserRoleQuery { UserId = response.Id, Roles = response.Roles } : null;
    }

    public async Task<UserRankQuery> GetRankAsync(int userId, string gameMode)
    {
        switch (gameMode)
        {
            case "osu":
            {
                var response = await _database
                    .From<User>()
                    .Select(x => new object[] { x.OsuRank })
                    .Where(x => x.Id == userId)
                    .Single();

                if (response != null)
                {
                    return new UserRankQuery { Rank = response.OsuRank };
                }

                break;
            }
            case "taiko":
            {
                var response = await _database
                    .From<User>()
                    .Select(x => new object[] { x.TaikoRank })
                    .Where(x => x.Id == userId)
                    .Single();

                if (response != null)
                {
                    return new UserRankQuery { Rank = response.TaikoRank };
                }

                break;
            }
            case "catch":
            {
                var response = await _database
                    .From<User>()
                    .Select(x => new object[] { x.CatchRank })
                    .Where(x => x.Id == userId)
                    .Single();

                if (response != null)
                {
                    return new UserRankQuery { Rank = response.CatchRank };
                }

                break;
            }
            case "mania":
            {
                var response = await _database
                    .From<User>()
                    .Select(x => new object[] { x.ManiaRank })
                    .Where(x => x.Id == userId)
                    .Single();

                if (response != null)
                {
                    return new UserRankQuery { Rank = response.ManiaRank };
                }

                break;
            }
            default:
            {
                throw new InvalidOperationException($"Game mode {gameMode} is not supported.");
            }
        }

        throw new NullReferenceException("Returned user is null.");
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

    public async Task UpdateUserBestCertificateAsync(int userId, string gameMode, int rank, int rankCertificateId)
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
