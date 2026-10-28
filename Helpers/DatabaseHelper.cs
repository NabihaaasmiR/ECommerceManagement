using Microsoft.Data.SqlClient;

namespace ECommerceManagement.Helpers;

public static class DatabaseHelper
{
    private const string ConnectionString =
        @"Server=.\SQLEXPRESS;Database=ECommerceDB;Trusted_Connection=True;TrustServerCertificate=True;";

    public static SqlConnection CreateConnection() => new SqlConnection(ConnectionString);
}