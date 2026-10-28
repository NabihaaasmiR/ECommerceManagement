using System;
using System.Collections.Generic;
using ECommerceManagement.Models;
using ECommerceManagement.Repositories;

namespace ECommerceManagement.Services;

public class CustomerService
{
    private readonly CustomerRepository _repository;
    public CustomerService(CustomerRepository repository) => _repository = repository;

    public int Register(Customer customer)
    {
        if (string.IsNullOrWhiteSpace(customer.FullName))
            throw new ArgumentException("Customer name is required.");
        if (string.IsNullOrWhiteSpace(customer.Email))
            throw new ArgumentException("Email is required.");

        return _repository.Add(customer);
    }

    public List<Customer> GetAll() => _repository.GetAll();
}