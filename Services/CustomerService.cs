using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerceManagement.Models;
using ECommerceManagement.Repositories;

namespace ECommerceManagement.Services;

public class CustomerService
{
    private readonly CustomerRepository _repository;
    public CustomerService(CustomerRepository repository) => _repository = repository;

    public async Task<int> RegisterAsync(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.FullName))
            throw new ArgumentException("Customer name is required.");
        if (string.IsNullOrWhiteSpace(customer.Email))
            throw new ArgumentException("Email is required.");

        return await _repository.AddAsync(customer);
    }

    public Task<List<Customer>> GetAllAsync() => _repository.GetAllAsync();
}