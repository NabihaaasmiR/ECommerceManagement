using System;
using System.Collections.Generic;

namespace ECommerceManagement.Models;

public class Order
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending";
    public List<OrderItem> OrderItems { get; set; } = new();
}