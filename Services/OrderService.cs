using Microsoft.Extensions.Logging;
using QiECommerceAPI.Models;
using QiECommerceAPI.Repositories;

namespace QiECommerceAPI.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<(bool success, string? errorMessage, Order? order)> CreateOrderAsync(
            string userId,
            IEnumerable<OrderItem> items)
        {
            var productIds = items.Select(i => i.ProductId).Distinct().ToList();
            var products = new List<Product>();

            // Validate stock
            foreach (var productId in productIds)
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    return (false, $"Product with ID {productId} not found.", null);
                }

                var totalRequested = items.Where(i => i.ProductId == productId).Sum(i => i.Quantity);
                if (product.Stock < totalRequested)
                {
                    return (false, $"Insufficient stock for '{product.Name}'. Current stock: {product.Stock}.", null);
                }
                products.Add(product);
            }

            // Deduct stock
            foreach (var product in products)
            {
                var totalRequested = items.Where(i => i.ProductId == product.Id).Sum(i => i.Quantity);
                product.Stock -= totalRequested;
                _productRepository.Update(product);
            }

            // Create order
            var order = new Order
            {
                CustomerId = userId,
                Status = "Processing",
                TrackingNumber = GenerateTrackingNumber()
            };

            await _orderRepository.AddOrderAsync(order);
            await _productRepository.SaveChangesAsync(); // get Order.Id

            // Create order items
            var orderItems = new List<OrderItem>();
            foreach (var item in items)
            {
                var product = products.First(p => p.Id == item.ProductId);
                orderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = product.Id,
                    Quantity = item.Quantity,
                    UnitPriceIQD = product.PriceIQD
                });
            }

            await _orderRepository.AddOrderItemsAsync(orderItems);
            await _orderRepository.SaveChangesAsync();

            // Log if stock is now below a threshold
            foreach (var p in products.Where(p => p.Stock < 5))
            {
                _logger.LogWarning($"Stock low for product '{p.Name}' (ID {p.Id}). Current stock={p.Stock}");
            }

            return (true, null, order);
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string? status,
            string? customerId)
        {
            return await _orderRepository.GetOrdersAsync(fromDate, toDate, status, customerId);
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string newStatus)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return false;

            order.Status = newStatus;
            await _orderRepository.SaveChangesAsync();
            return true;
        }

        private string GenerateTrackingNumber()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
        }
    }
}
