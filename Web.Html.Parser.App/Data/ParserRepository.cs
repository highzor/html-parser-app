using Dapper;
using Npgsql;
using System.Collections.Generic;
using System.Data;

namespace Web.Html.Parser.App.Data;

public interface IParserRepository
{
    Task<Guid> AddGetGame(int gameWebId, string gameName);
    Task<Guid> AddGameItem(Guid gameId, string gameItemTitle);
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

    public async Task<Guid> AddGetGame(int gameWebId, string gameName)
    {
        using var connection = createConnection();

        return await connection.QueryFirstOrDefaultAsync<Guid>(@"
                WITH source AS
                (
	                SELECT 
		                 id,
		                 game_web_id
	                FROM game
	                WHERE game_web_id = @gameWebId
                ),
                target AS 
                (
	                INSERT INTO game
	                (
		                id,
		                game_web_id,
		                title
	                )
	                SELECT 
		                 uuid_generate_v4(),
		                 @gameWebId, 
		                 @gameName
	                WHERE NOT EXISTS
	                (
		                SELECT 1 FROM source
	                )
	                RETURNING id
                )
                SELECT 
	                 id
                FROM target 
                UNION ALL
                SELECT 
	                 id
                FROM source", new
        {
            gameWebId,
            gameName
        });
    }

    public async Task<Guid> AddGameItem(Guid gameId, string gameItemTitle)
    {
        using var connection = createConnection();

        return await connection.QueryFirstOrDefaultAsync<Guid>(@"
                INSERT INTO game_item
                (
	                id,
	                game_id,
	                title
                )
                SELECT 
	                 uuid_generate_v4(),
	                 @gameId ,
	                 @gameItemTitle
                WHERE NOT EXISTS
                (
	                SELECT 
		                 title
	                FROM game_item
	                WHERE game_id = @gameId AND title = @gameItemTitle
                )", new
        {
            gameId,
            gameItemTitle
        });
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