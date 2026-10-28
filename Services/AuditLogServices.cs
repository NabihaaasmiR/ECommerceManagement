using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Repositories;

namespace ECommerceManagement.Services;

public class AuditLogService
{
    private readonly AuditLogRepository _repository;
    public AuditLogService(AuditLogRepository repository) => _repository = repository;

    public Task LogActionAsync(string action, string details, SqlConnection connection, SqlTransaction transaction)
    {
        return _repository.LogAsync(action, details, connection, transaction);
    }
}