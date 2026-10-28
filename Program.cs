using System;
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

        var categoryService = new CategoryService(categoryRepo);
        var productService = new ProductService(productRepo);
        var customerService = new CustomerService(customerRepo);

        while (true)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("  E-COMMERCE SYSTEM (VALIDATION VERSION) ");
            Console.WriteLine("========================================");
            Console.WriteLine("1. Manage Categories");
            Console.WriteLine("2. Manage Products");
            Console.WriteLine("3. Manage Customers");
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
                Console.WriteLine($"\n[Validation Error]: {ex.Message}");
            }
            catch (SqlException ex)
            {
                // Unique constraint violations (e.g., duplicate category name or duplicate email)
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
}