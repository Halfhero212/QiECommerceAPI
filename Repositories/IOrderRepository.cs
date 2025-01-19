using QiECommerceAPI.Models;

namespace QiECommerceAPI.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<IEnumerable<Order>> GetOrdersAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string? status,
            string? customerId);

        Task AddOrderAsync(Order order);
        Task AddOrderItemsAsync(IEnumerable<OrderItem> items);

        Task SaveChangesAsync();
    }
}
