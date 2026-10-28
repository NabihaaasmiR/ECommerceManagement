using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;

namespace ECommerceManagement.Repositories;

public class CategoryRepository
{
    public async Task<int> AddAsync(Category category)
    {
        const string sql = """
            INSERT INTO Categories (CategoryName)
            VALUES (@CategoryName);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using var connection = DatabaseHelper.CreateConnection();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = category.CategoryName;

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<List<Category>> GetAllAsync()
    {
        const string sql = "SELECT CategoryId, CategoryName, IsActive FROM Categories ORDER BY CategoryId";
        var list = new List<Category>();

        await using var connection = DatabaseHelper.CreateConnection();
        await using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Category
            {
                CategoryId = reader.GetInt32(0),
                CategoryName = reader.GetString(1),
                IsActive = reader.GetBoolean(2)
            });
        }
        return list;
    }
}