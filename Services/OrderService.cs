using System.Threading.Tasks;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;
using ECommerceManagement.Repositories;

namespace ECommerceManagement.Services;

public class OrderService
{
    private readonly OrderRepository _repository;
    public OrderService(OrderRepository repository) => _repository = repository;

    public async Task<int> PlaceOrderAsync(Order order)
    {
        if (order.CustomerId <= 0)
            throw new BusinessRuleException("A valid Customer ID is required to place an order.");
        if (order.OrderItems == null || order.OrderItems.Count == 0)
            throw new BusinessRuleException("An order must contain at least one item.");

        foreach (var item in order.OrderItems)
        {
            if (item.Quantity <= 0)
                throw new BusinessRuleException("Order item quantity must be greater than zero.");
            if (item.UnitPrice <= 0)
                throw new BusinessRuleException("Order item unit price must be greater than zero.");
        }

        return await _repository.CreateOrderAsync(order);
    }
}