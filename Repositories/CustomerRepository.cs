using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;

namespace ECommerceManagement.Repositories;

public class CustomerRepository
{
    public async Task<int> AddAsync(Customer customer)
    {
        const string sql = """
            INSERT INTO Customers (FullName, Email, Phone)
            VALUES (@FullName, @Email, @Phone);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        await using var connection = DatabaseHelper.CreateConnection();
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = customer.FullName;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = customer.Email;
        command.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = customer.Phone;

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<List<Customer>> GetAllAsync()
    {
        const string sql = "SELECT CustomerId, FullName, Email, Phone FROM Customers ORDER BY CustomerId";
        var list = new List<Customer>();

        await using var connection = DatabaseHelper.CreateConnection();
        await using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            list.Add(new Customer
            {
                CustomerId = reader.GetInt32(0),
                FullName = reader.GetString(1),
                Email = reader.GetString(2),
                Phone = reader.GetString(3)
            });
        }
        return list;
    }
}