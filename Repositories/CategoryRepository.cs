using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;

namespace ECommerceManagement.Repositories;

public class CategoryRepository
{
    public int Add(Category category)
    {
        const string sql = """
            INSERT INTO Categories (CategoryName)
            VALUES (@CategoryName);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = DatabaseHelper.CreateConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@CategoryName", SqlDbType.NVarChar, 100).Value = category.CategoryName;

        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public List<Category> GetAll()
    {
        const string sql = "SELECT CategoryId, CategoryName, IsActive FROM Categories ORDER BY CategoryId";
        var list = new List<Category>();

        using var connection = DatabaseHelper.CreateConnection();
        using var command = new SqlCommand(sql, connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
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