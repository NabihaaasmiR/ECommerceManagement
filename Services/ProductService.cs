using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceManagement.Models;
using ECommerceManagement.Repositories;

namespace ECommerceManagement.Services;

public class ProductService
{
    private readonly ProductRepository _repository;
    public ProductService(ProductRepository repository) => _repository = repository;

    public async Task<int> AddAsync(Product product)
    {
        if (string.IsNullOrWhiteSpace(product.ProductName))
            throw new ArgumentException("Product name is required.");
        if (product.Price <= 0)
            throw new ArgumentException("Price must be greater than zero.");
        if (product.StockQuantity < 0)
            throw new ArgumentException("Stock cannot be negative.");

        return await _repository.AddAsync(product);
    }

    public Task<List<Product>> GetAllAsync() => _repository.GetAllAsync();
}