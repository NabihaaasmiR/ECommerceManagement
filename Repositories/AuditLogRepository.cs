using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace ECommerceManagement.Repositories;

public class AuditLogRepository
{
    public async Task LogAsync(string action, string details, SqlConnection connection, SqlTransaction transaction)
    {
        const string sql = """
            INSERT INTO AuditLogs (Action, Details)
            VALUES (@Action, @Details);
            """;

        await using var cmd = new SqlCommand(sql, connection, transaction);
        cmd.Parameters.Add("@Action", SqlDbType.NVarChar, 100).Value = action;
        cmd.Parameters.Add("@Details", SqlDbType.NVarChar, -1).Value = details;

        await cmd.ExecuteNonQueryAsync();
    }
}