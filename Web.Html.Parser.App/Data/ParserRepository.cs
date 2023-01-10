using Dapper;
using Npgsql;
using System.Collections.Generic;
using System.Data;

namespace Web.Html.Parser.App.Data;

public interface IParserRepository
{
    Task<Guid> AddGetGame(int gameWebId, string gameName);
    Task<Guid> AddGameItem(Guid gameId, string gameItemTitle);
    Task<Guid> AddGetUser(int userWebId, string userName);
    Task<Guid> AddGetItem(Guid userId, Guid gameId, string description);
    Task<Guid> AddGetItemPrice(Guid itemId, double price, bool isSingle, int count);
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
	                WHERE 
                        game_web_id = @gameWebId
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
	                 @gameId,
	                 @gameItemTitle
                WHERE NOT EXISTS
                (
	                SELECT 
		                 title
	                FROM game_item
	                WHERE 
                        game_id = @gameId 
                        AND title = @gameItemTitle
                )", new
        {
            gameId,
            gameItemTitle
        });
    }

    public async Task<Guid> AddGetUser(int userWebId, string userName)
    {
        using var connection = createConnection();

        return await connection.QueryFirstOrDefaultAsync<Guid>(@"
               WITH source AS
                (
	                SELECT 
		                 id,
		                 user_web_id
	                FROM users
	                WHERE 
                        user_web_id = @userWebId
                ),
                target AS 
                (
	                INSERT INTO users
	                (
		                id,
		                user_web_id,
		                user_name
	                )
	                SELECT 
		                 uuid_generate_v4(),
		                 @userWebId, 
		                 @userName
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
            userWebId,
            userName
        });
    }

    public async Task<Guid> AddGetItem(Guid userId, Guid gameId, string description)
    {
        using var connection = createConnection();

        return await connection.QueryFirstOrDefaultAsync<Guid>(@"
                WITH source AS
                (
	                SELECT 
		                 id,
		                 seller_id,
                         game_id,
                         description
	                FROM item
	                WHERE 
                        seller_id = @userId
                        AND game_id = @gameId
                        AND description = @description
                ),
                target AS 
                (
	                INSERT INTO item
	                (
		                id,
		                seller_id,
                        game_id,
                        description
	                )
	                SELECT 
		                 uuid_generate_v4(),
		                 @userId, 
		                 @gameId,
                         @description
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
            userId,
            gameId,
            description
        });
    }

    public async Task<Guid> AddGetItemPrice(Guid itemId, double price, bool isSingle, int count)
    {
        using var connection = createConnection();

        return await connection.QueryFirstOrDefaultAsync<Guid>(@"  
                INSERT INTO item_price
                (
	                id,
	                item_id,
                    price,
                    is_single,
                    count,
                    date_time_update
                )
                VALUES 
                (
	                uuid_generate_v4(),
	                @itemId, 
	                @price,
                    @isSingle,
                    @count,
                    now()
                )
                RETURNING id", new
        {
            itemId,
            price,
            isSingle,
            count
        });
    }

    private IDbConnection createConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}