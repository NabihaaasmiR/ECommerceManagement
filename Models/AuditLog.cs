using System;

namespace ECommerceManagement.Models;

public class AuditLog
{
    public int LogId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}