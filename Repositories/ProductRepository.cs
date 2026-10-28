using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;

namespace ECommerceManagement.Repositories;

public class ProductRepository
{
    public int Add(Product product)
    {
        const string sql = """
            INSERT INTO Products (ProductName, CategoryId, Price, StockQuantity)
            VALUES (@ProductName, @CategoryId, @Price, @StockQuantity);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = DatabaseHelper.CreateConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@ProductName", SqlDbType.NVarChar, 150).Value = product.ProductName;
        command.Parameters.Add("@CategoryId", SqlDbType.Int).Value = product.CategoryId;
        command.Parameters.Add("@Price", SqlDbType.Decimal).Value = product.Price;
        command.Parameters.Add("@StockQuantity", SqlDbType.Int).Value = product.StockQuantity;

        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public List<Product> GetAll()
    {
        const string sql = """
            SELECT p.ProductId, p.ProductName, p.CategoryId, c.CategoryName, p.Price, p.StockQuantity
            FROM Products p
            INNER JOIN Categories c ON c.CategoryId = p.CategoryId
            ORDER BY p.ProductId
            """;

        var list = new List<Product>();
        using var connection = DatabaseHelper.CreateConnection();
        using var command = new SqlCommand(sql, connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
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