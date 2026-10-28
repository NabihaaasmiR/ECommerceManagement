namespace ECommerceManagement.Models;

public class Product
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty; // For JOIN queries
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}