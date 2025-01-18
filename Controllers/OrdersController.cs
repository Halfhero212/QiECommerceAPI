using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QiECommerceAPI.Services;
using QiECommerceAPI.Models;

namespace QiECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        // POST: /api/orders (Customer only)
        [HttpPost]
        [Authorize(Policy = "CustomerOnly")]
        public async Task<IActionResult> CreateOrder([FromBody] List<OrderItem> items)
        {
            var userId = User.Identity?.Name ?? "Anonymous";
            _logger.LogInformation($"User {userId} is creating an order with {items.Count} items.");

            var (success, errorMessage, order) = await _orderService.CreateOrderAsync(userId, items);
            if (!success)
            {
                return BadRequest(errorMessage);
            }

            return Ok(new { OrderId = order!.Id, TrackingNumber = order.TrackingNumber });
        }

        // GET: /api/orders (Admin only)
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetOrders(
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] string? status,
            [FromQuery] string? customerId)
        {
            var orders = await _orderService.GetOrdersAsync(fromDate, toDate, status, customerId);
            return Ok(orders);
        }

        // GET: /api/orders/{id} (Admin or Owner)
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _orderService.GetOrderAsync(id);
            if (order == null) return NotFound("Order not found.");

            var isAdmin = User.IsInRole("Admin");
            var isOwner = order.CustomerId == User.Identity?.Name;
            if (!isAdmin && !isOwner) return Forbid();

            return Ok(order);
        }

        // PUT: /api/orders/{id}/status (Admin only)
        [HttpPut("{id}/status")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string newStatus)
        {
            var success = await _orderService.UpdateOrderStatusAsync(id, newStatus);
            if (!success)
                return NotFound("Order not found.");

            return NoContent();
        }
    }
}
