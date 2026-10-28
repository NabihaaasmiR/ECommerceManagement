using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;

namespace ECommerceManagement.Repositories;

public class ProductRepository
{
    public async Task<int> AddAsync(Product product)
    {
        const string sql = """
            INSERT INTO Products (ProductName, CategoryId, Price, StockQuantity)
            VALUES (@ProductName, @CategoryId, @Price, @StockQuantity);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using var connection = DatabaseHelper.CreateConnection();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@ProductName", SqlDbType.NVarChar, 150).Value = product.ProductName;
        command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = product.CategoryId;
        command.Parameters.Add("@Price", SqlDbType.Decimal).Value = product.Price;
        command.Parameters.Add("@StockQuantity", SqlDbType.Int).Value = product.StockQuantity;

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<List<Product>> GetAllAsync()
    {
        const string sql = """
            SELECT p.ProductId, p.ProductName, p.CategoryId, c.CategoryName, p.Price, p.StockQuantity
            FROM Products p
            INNER JOIN Categories c ON c.CategoryId = p.CategoryId
            ORDER BY p.ProductId
            """;

        var list = new List<Product>();
        await using var connection = DatabaseHelper.CreateConnection();
        await using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Product
            {
                ProductId = reader.GetInt32(0),
                ProductName = reader.GetString(1),
                CategoryId = reader.GetInt32(2),
                CategoryName = reader.GetString(3),
                Price = reader.GetDecimal(4),
                StockQuantity = reader.GetInt32(5)
            });
        }
        return list;
    }
}