using System;
using System.Collections.Generic;
using ECommerceManagement.Models;
using ECommerceManagement.Repositories;

namespace ECommerceManagement.Services;

public class CategoryService
{
    private readonly CategoryRepository _repository;
    public CategoryService(CategoryRepository repository) => _repository = repository;

    public int Add(Category category)
    {
        if (string.IsNullOrWhiteSpace(category.CategoryName))
            throw new ArgumentException("Category name is required.");

        return _repository.Add(category);
    }

    public List<Category> GetAll() => _repository.GetAll();
}