using Dapper;
using Npgsql;
using System.Data;

namespace Web.Html.Parser.App.Data;

public interface IParserRepository
{
    Task<Guid> AddUpdateGame(int gameWebId, string gameName);
    Task<Guid> AddUpdateGameItem(Guid gameId, string gameItemTitle);
    Task<Guid> AddUpdateUser(int userWebId, string userName);
    Task<Guid> AddUpdateItem(Guid userId, Guid gameId, string description);
    Task<Guid> AddUpdateItemPrice(Guid itemId, double price, int count);
}

public class ParserRepository : IParserRepository
{
    private readonly string _connectionString;

    public ParserRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Guid> AddUpdateGame(int gameWebId, string gameName)
    {
        using var connection = createConnection();
        return await connection.QueryFirstOrDefaultAsync<Guid>(@"
            INSERT INTO game
            (
                id,
                game_web_id,
                title
            )
            VALUES
            (
                uuid_generate_v4(),
                @gameWebId,
                @gameName
            )
            RETURNING id", new
        {
            gameWebId,
            gameName
        });
    }

    public async Task<Guid> AddUpdateGameItem(Guid gameId, string gameItemTitle)
    {
        return Guid.Empty;
    }

    public async Task<Guid> AddUpdateUser(int userWebId, string userName)
    {
        return Guid.Empty;
    }

    public async Task<Guid> AddUpdateItem(Guid userId, Guid gameId, string description)
    {
        return Guid.Empty;
    }

    public async Task<Guid> AddUpdateItemPrice(Guid itemId, double price, int count)
    {
        return Guid.Empty;
    }

    private IDbConnection createConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}