using QiECommerceAPI.Models;

namespace QiECommerceAPI.Services
{
    public interface IOrderService
    {
        Task<(bool success, string? errorMessage, Order? order)> CreateOrderAsync(
            string userId,
            IEnumerable<OrderItem> items);

        Task<IEnumerable<Order>> GetOrdersAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string? status,
            string? customerId);

        Task<Order?> GetOrderAsync(int id);
        Task<bool> UpdateOrderStatusAsync(int id, string newStatus);
    }
}
