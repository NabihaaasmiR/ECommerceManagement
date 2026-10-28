using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;
using ECommerceManagement.Repositories;
using ECommerceManagement.Services;

namespace ECommerceManagement;

class Program
{
    static async Task Main(string[] args)
    {
        var categoryRepo = new CategoryRepository();
        var productRepo = new ProductRepository();
        var customerRepo = new CustomerRepository();
        var orderRepo = new OrderRepository();

        var categoryService = new CategoryService(categoryRepo);
        var productService = new ProductService(productRepo);
        var customerService = new CustomerService(customerRepo);
        var orderService = new OrderService(orderRepo);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("  E-COMMERCE SYSTEM (FULL ARCHITECTURE) ");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Manage Categories");
            Console.WriteLine("2. Manage Products");
            Console.WriteLine("3. Manage Customers");
            Console.WriteLine("4. Place Order");
            Console.WriteLine("5. View Audit Logs");
            Console.WriteLine("0. Exit");
            Console.WriteLine("========================================");
            Console.Write("Enter choice: ");

            string? choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        await ManageCategoriesAsync(categoryService);
                        break;
                    case "2":
                        await ManageProductsAsync(productService);
                        break;
                    case "3":
                        await ManageCustomersAsync(customerService);
                        break;
                    case "4":
                        await PlaceOrderInteractiveAsync(orderService, productService);
                        break;
                    case "5":
                        await ViewAuditLogsAsync();
                        break;
                    case "0":
                        Console.WriteLine("Exiting application...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
            }
            catch (BusinessRuleException ex)
            {
                Console.WriteLine($"\n[Validation/Business Error]: {ex.Message}");
            }
            catch (SqlException ex)
            {
                if (ex.Number == 2627 || ex.Number == 2601)
                    Console.WriteLine("\n[Database Error]: A record with this name or email already exists.");
                else
                    Console.WriteLine($"\n[Database Error]: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[Unexpected Error]: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }

    private static async Task ManageCategoriesAsync(CategoryService service)
    {
        Console.Clear();
        Console.WriteLine("--- CATEGORIES ---");
        Console.WriteLine("1. Add Category\n2. View All Categories");
        Console.Write("Choice: ");
        if (Console.ReadLine() == "1")
        {
            Console.Write("Category Name: ");
            string name = Console.ReadLine() ?? "";
            int id = await service.AddAsync(new Category { CategoryName = name });
            Console.WriteLine($"Category added with ID: {id}");
        }
        else
        {
            var list = await service.GetAllAsync();
            foreach (var c in list)
                Console.WriteLine($"{c.CategoryId,-5} | {c.CategoryName,-20} | Active: {c.IsActive}");
        }
    }

    private static async Task ManageProductsAsync(ProductService service)
    {
        Console.Clear();
        Console.WriteLine("--- PRODUCTS ---");
        Console.WriteLine("1. Add Product\n2. View All Products");
        Console.Write("Choice: ");
        if (Console.ReadLine() == "1")
        {
            Console.Write("Product Name: "); string name = Console.ReadLine() ?? "";
            Console.Write("Category ID: "); int catId = int.Parse(Console.ReadLine() ?? "1");
            Console.Write("Price: "); decimal price = decimal.Parse(Console.ReadLine() ?? "0");
            Console.Write("Stock: "); int stock = int.Parse(Console.ReadLine() ?? "0");

            int id = await service.AddAsync(new Product
            {
                ProductName = name,
                CategoryId = catId,
                Price = price,
                StockQuantity = stock
            });
            Console.WriteLine($"Product added with ID: {id}");
        }
        else
        {
            var list = await service.GetAllAsync();
            foreach (var p in list)
                Console.WriteLine($"{p.ProductId,-5} | {p.ProductName,-20} | {p.CategoryName,-15} | Rs.{p.Price:N2} | Stock: {p.StockQuantity}");
        }
    }

    private static async Task ManageCustomersAsync(CustomerService service)
    {
        Console.Clear();
        Console.WriteLine("--- CUSTOMERS ---");
        Console.WriteLine("1. Register Customer\n2. View All Customers");
        Console.Write("Choice: ");
        if (Console.ReadLine() == "1")
        {
            Console.Write("Full Name: "); string name = Console.ReadLine() ?? "";
            Console.Write("Email: "); string email = Console.ReadLine() ?? "";
            Console.Write("Phone: "); string phone = Console.ReadLine() ?? "";

            int id = await service.RegisterAsync(new Customer
            {
                FullName = name,
                Email = email,
                Phone = phone
            });
            Console.WriteLine($"Customer registered with ID: {id}");
        }
        else
        {
            var list = await service.GetAllAsync();
            foreach (var c in list)
                Console.WriteLine($"{c.CustomerId,-5} | {c.FullName,-20} | {c.Email,-25} | {c.Phone}");
        }
    }

    private static async Task PlaceOrderInteractiveAsync(OrderService orderService, ProductService productService)
    {
        Console.Clear();
        Console.WriteLine("--- PLACE NEW ORDER ---");

        Console.Write("Enter Customer ID: ");
        if (!int.TryParse(Console.ReadLine(), out int customerId))
        {
            Console.WriteLine("Invalid Customer ID.");
            return;
        }

        var orderItems = new List<OrderItem>();
        decimal totalAmount = 0;

        while (true)
        {
            Console.Write("\nEnter Product ID to add (or type '0' to finish): ");
            string input = Console.ReadLine() ?? "";
            if (input == "0") break;

            if (!int.TryParse(input, out int productId))
            {
                Console.WriteLine("Invalid Product ID.");
                continue;
            }

            Console.Write("Enter Quantity: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Invalid quantity.");
                continue;
            }

            var products = await productService.GetAllAsync();
            var product = products.Find(p => p.ProductId == productId);
            if (product == null)
            {
                Console.WriteLine("Product not found.");
                continue;
            }

            orderItems.Add(new OrderItem
            {
                ProductId = productId,
                Quantity = quantity,
                UnitPrice = product.Price
            });

            totalAmount += product.Price * quantity;
            Console.WriteLine($"Added {quantity} x {product.ProductName} to order.");
        }

        if (orderItems.Count == 0)
        {
            Console.WriteLine("Order cancelled: No items selected.");
            return;
        }

        var newOrder = new Order
        {
            CustomerId = customerId,
            TotalAmount = totalAmount,
            Status = "Completed",
            OrderItems = orderItems
        };

        int orderId = await orderService.PlaceOrderAsync(newOrder);
        Console.WriteLine($"\nSuccess! Order #{orderId} placed successfully with Total Amount: Rs.{totalAmount:N2}. Stock updated atomically.");
    }

    private static async Task ViewAuditLogsAsync()
    {
        Console.Clear();
        Console.WriteLine("--- SYSTEM AUDIT LOGS ---");
        const string sql = "SELECT LogId, Action, Details, CreatedAt FROM AuditLogs ORDER BY CreatedAt DESC";

        await using var connection = DatabaseHelper.CreateConnection();
        await using var command = new SqlCommand(sql, connection);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        bool hasRows = false;
        while (await reader.ReadAsync())
        {
            hasRows = true;
            int id = reader.GetInt32(0);
            string action = reader.GetString(1);
            string details = reader.GetString(2);
            DateTime createdAt = reader.GetDateTime(3);

            Console.WriteLine($"[{createdAt:yyyy-MM-dd HH:mm:ss}] [ID: {id}] {action} -> {details}");
        }

        if (!hasRows)
        {
            Console.WriteLine("No audit logs found in the database.");
        }
    }
}