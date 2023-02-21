using Dapper;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using Web.Html.Parser.App.Data;
using Web.Html.Parser.App.Models;

namespace Web.Html.Parser.App.Data;

public interface ICalculatorRepository
{
    Task<IList<ItemPriceModel>> GetItemsPrice();
}

public class CalculatorRepository : ICalculatorRepository
{
    private readonly string _connectionString;

    public CalculatorRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IList<ItemPriceModel>> GetItemsPrice()
    {
        using var connection = createConnection();

        return (await connection.QueryAsync<ItemPriceModel>(@"
                SELECT 
	                 *
                FROM 
	                (
	                 SELECT
		                  id as Id, 
                          item_id as ItemId, 
                          price as Price, 
                          count as Count, 
                          is_single as IsSingle, 
                          date_time_update as DateTimeUpdate
	                 FROM item_price
	                 GROUP BY id
	                 ORDER BY item_id, date_time_update
	                ) AS sub
                ORDER BY ItemId")).ToList();
    }

    private IDbConnection createConnection()
    {
        return new NpgsqlConnection(_connectionString);
    }
}