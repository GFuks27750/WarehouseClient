using System.Data;
using Microsoft.Data.SqlClient;
using WarehouseClient.DTOs;

namespace WarehouseClient.Services;

public interface IDbService
{
    Task<GetProductDTO?> GetProductDTOAsync(int id);
}
public class DbService(IConfiguration configuration) : IDbService
{
    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection(configuration.GetConnectionString("Default"));
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync();
        }

        return connection;
    }

    public async Task<GetProductDTO?> GetProductDTOAsync(int id)
    {
        await using var connection = await GetConnection();
        var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = """
                              SELECT IdProduct, Name, Description,Price
                              FROM product
                              WHERE IdProduct = @id
                              """;
        command.Parameters.AddWithValue("@id", id);
        var reader = await command.ExecuteReaderAsync();
        
        if(!reader.HasRows) return null;
        await reader.ReadAsync();
        var result = new GetProductDTO(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetDecimal(3)
        );
        return result;
    }
}