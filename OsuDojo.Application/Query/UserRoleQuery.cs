namespace OsuDojo.Application.Query;

public class UserRoleQuery
{
    public int UserId { get; init; }
    public required string[] Roles { get; init; }
}
