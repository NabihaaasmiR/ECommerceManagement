using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceManagement.Models;
using ECommerceManagement.Repositories;

namespace ECommerceManagement.Services;

public class CategoryService
{
    private readonly CategoryRepository _repository;
    public CategoryService(CategoryRepository repository) => _repository = repository;

    public async Task<int> AddAsync(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.CategoryName))
            throw new ArgumentException("Category name is required.");

        return await _repository.AddAsync(category);
    }

    public Task<List<Category>> GetAllAsync() => _repository.GetAllAsync();
}