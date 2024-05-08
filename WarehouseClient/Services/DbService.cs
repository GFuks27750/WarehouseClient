using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using Warehouse.DTOs;
using Warehouse.Models;

namespace Warehouse.Services
{
    public interface IDbService
    {
        Task<List<ProductWarehouseDTO>> GetWarehouseProducts();
        Task<int?> CreateProductWarehouse(CreateProductWarehouseRequest request);
    }

    public class DbService : IDbService
    {
        private readonly IConfiguration _configuration;

        public DbService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<SqlConnection> GetConnection()
        {
            var sqlConnection = new SqlConnection(_configuration.GetConnectionString("Default"));
            if (sqlConnection.State != ConnectionState.Open) await sqlConnection.OpenAsync();
            return sqlConnection;
        }

        public async Task<List<ProductWarehouseDTO>> GetWarehouseProducts()
        {
            await using var sqlConnection = await GetConnection();
            var response = new List<ProductWarehouseDTO>();
            var command = new SqlCommand("SELECT * FROM Product_Warehouse", sqlConnection);

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                response.Add(new ProductWarehouseDTO(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetInt32(2),
                        reader.GetInt32(3),
                        reader.GetInt32(4),
                        reader.GetDouble(5),
                        reader.GetDateTime(6)
                    )
                );
            }

            return response;
        }

        public async Task<int?> CreateProductWarehouse(CreateProductWarehouseRequest request)
        {
            await using var sqlConnection = await GetConnection();

            var c1 = new SqlCommand("SELECT * FROM PRODUCT WHERE IdProduct = @id", sqlConnection);
            c1.Parameters.AddWithValue("@id", request.IdProduct);
            await using var reader1 = await c1.ExecuteReaderAsync();
            if (!reader1.HasRows)
            {
                return null;
            }
            await reader1.CloseAsync();
            
            var c2 = new SqlCommand("SELECT * FROM WAREHOUSE WHERE IdWarehouse = @id", sqlConnection);
            c2.Parameters.AddWithValue("@id", request.IdWarehouse);
            await using var reader2 = await c2.ExecuteReaderAsync();
            if (!reader2.HasRows)
            {
                return null;
            }
            await reader2.CloseAsync();

            if (request.Amount <= 0)
            {
                return null;
            }

            var c3 = new SqlCommand("SELECT IdOrder FROM [ORDER] WHERE IdProduct = @1 AND Amount = @2 AND CreatedAt < @3", sqlConnection);
            c3.Parameters.AddWithValue("@1", request.IdProduct);
            c3.Parameters.AddWithValue("@2", request.Amount);
            c3.Parameters.AddWithValue("@3", request.CreatedAt);
            await using var reader3 = await c3.ExecuteReaderAsync();
            if (!reader3.HasRows)
            {
                return null;
            }

            await reader3.ReadAsync();
            var idOrder = reader3.GetInt32(0);
            await reader3.CloseAsync();

            var c4 = new SqlCommand("SELECT * FROM PRODUCT_WAREHOUSE WHERE IdOrder = @id", sqlConnection);
            c4.Parameters.AddWithValue("@id", idOrder);
            await using var reader4 = await c4.ExecuteReaderAsync();
            if (reader4.HasRows)
            {
                return null;
            }
            await reader4.CloseAsync();

            var c5 = new SqlCommand("UPDATE [ORDER] SET FulfilledAt = GETDATE() WHERE IdOrder = @1", sqlConnection);
            c5.Parameters.AddWithValue("@1", idOrder);
            await c5.ExecuteNonQueryAsync();

            var c6 = new SqlCommand("SELECT Price FROM Product WHERE IdProduct = @1", sqlConnection);
            c6.Parameters.AddWithValue("@1", request.IdProduct);
            await using var reader6 = await c6.ExecuteReaderAsync();
            await reader6.ReadAsync();
            var price = reader6.GetDecimal(0);
            await reader6.CloseAsync();

            var c7 = new SqlCommand("INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES (@1, @2, @3, @4, @5, GETDATE())", sqlConnection);
            c7.Parameters.AddWithValue("@1", request.IdWarehouse);
            c7.Parameters.AddWithValue("@2", request.IdProduct);
            c7.Parameters.AddWithValue("@3", idOrder);
            c7.Parameters.AddWithValue("@4", request.Amount);
            c7.Parameters.AddWithValue("@5", request.Amount * price);
            await c7.ExecuteNonQueryAsync();

            var c8 = new SqlCommand("SELECT IdProductWarehouse FROM Product_Warehouse WHERE IdOrder = @1", sqlConnection);
            c8.Parameters.AddWithValue("@1", idOrder);
            await using var reader8 = await c8.ExecuteReaderAsync();
            await reader8.ReadAsync();
            var id = reader8.GetInt32(0);
            await reader8.CloseAsync();
            
            return id;
        }
    }
}
