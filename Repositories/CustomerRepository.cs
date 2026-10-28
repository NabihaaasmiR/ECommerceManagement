using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;

namespace ECommerceManagement.Repositories;

public class CustomerRepository
{
    public int Add(Customer customer)
    {
        const string sql = """
            INSERT INTO Customers (FullName, Email, Phone)
            VALUES (@FullName, @Email, @Phone);
            SELECT CAST(SCOPE_IDENTITY() AS INT);
            """;

        using var connection = DatabaseHelper.CreateConnection();
        using var command = new SqlCommand(sql, connection);
        command.Parameters.Add("@FullName", SqlDbType.NVarChar, 100).Value = customer.FullName;
        command.Parameters.Add("@Email", SqlDbType.NVarChar, 255).Value = customer.Email;
        command.Parameters.Add("@Phone", SqlDbType.NVarChar, 15).Value = customer.Phone;

        connection.Open();
        return Convert.ToInt32(command.ExecuteScalar());
    }

    public List<Customer> GetAll()
    {
        const string sql = "SELECT CustomerId, FullName, Email, Phone FROM Customers ORDER BY CustomerId";
        var list = new List<Customer>();

        using var connection = DatabaseHelper.CreateConnection();
        using var command = new SqlCommand(sql, connection);

        connection.Open();
        using var reader = command.ExecuteReader();
        while (reader.Read())
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