using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using QiECommerceAPI.Data;

namespace QiECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(ApplicationDbContext dbContext, ILogger<ReportsController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        // GET: /api/reports/sales?from=2025-01-01&to=2025-01-07
        [HttpGet("sales")]
        public async Task<IActionResult> GetSalesReport([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var orders = await _dbContext.Orders
                .Where(o => o.OrderDate >= from && o.OrderDate <= to)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            var totalRevenueIQD = orders.Sum(o => o.OrderItems.Sum(oi => oi.UnitPriceIQD * oi.Quantity));
            var totalOrders = orders.Count;
            var topSellingProducts = orders
                .SelectMany(o => o.OrderItems)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new 
                {
                    ProductId = g.Key,
                    QuantitySold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToList();

            _logger.LogInformation($"Sales Report from {from:yyyy-MM-dd} to {to:yyyy-MM-dd}");

            return Ok(new 
            {
                FromDate = from,
                ToDate = to,
                TotalOrders = totalOrders,
                TotalRevenueIQD = totalRevenueIQD,
                TopSellingProducts = topSellingProducts
            });
        }
    }
}
