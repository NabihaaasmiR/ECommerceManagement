using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ECommerceManagement.Helpers;
using ECommerceManagement.Models;

namespace ECommerceManagement.Repositories;

public class OrderRepository
{
    public async Task<int> CreateOrderAsync(Order order)
    {
        await using var connection = DatabaseHelper.CreateConnection();
        await connection.OpenAsync();

        // Begin transaction for multi-table atomic operation
        await using SqlTransaction transaction = connection.BeginTransaction();

        try
        {
            // 1. Insert Order Header
            const string orderSql = """
                INSERT INTO Orders (CustomerId, TotalAmount, Status)
                VALUES (@CustomerId, @TotalAmount, @Status);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

            int orderId;
            await using (var cmd = new SqlCommand(orderSql, connection, transaction))
            {
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = order.CustomerId;
                cmd.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = order.TotalAmount;
                cmd.Parameters.Add("@Status", SqlDbType.NVarChar, 50).Value = order.Status;

                var result = await cmd.ExecuteScalarAsync();
                orderId = Convert.ToInt32(result);
            }

            // 2. Insert Order Items & Update Product Stock
            foreach (var item in order.OrderItems)
            {
                const string itemSql = """
                    INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice)
                    VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);
                    """;

                await using (var cmd = new SqlCommand(itemSql, connection, transaction))
                {
                    cmd.Parameters.Add("@OrderId", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;
                    cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;
                    cmd.Parameters.Add("@UnitPrice", SqlDbType.Decimal).Value = item.UnitPrice;
                    await cmd.ExecuteNonQueryAsync();
                }

                // Reduce stock
                const string stockSql = """
                    UPDATE Products 
                    SET StockQuantity = StockQuantity - @Quantity 
                    WHERE ProductId = @ProductId AND StockQuantity >= @Quantity;
                    """;

                await using (var cmd = new SqlCommand(stockSql, connection, transaction))
                {
                    cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = item.Quantity;
                    cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = item.ProductId;

                    int rowsAffected = await cmd.ExecuteNonQueryAsync();
                    if (rowsAffected == 0)
                    {
                        // If stock update failed, throw exception to trigger transaction rollback
                        throw new BusinessRuleException($"Insufficient stock or invalid product ID for Product ID {item.ProductId}.");
                    }
                }
            }

            // Commit transaction if all operations succeed
            await transaction.CommitAsync();
            return orderId;
        }
        catch
        {
            // Rollback transaction on any failure
            await transaction.RollbackAsync();
            throw;
        }
    }
}