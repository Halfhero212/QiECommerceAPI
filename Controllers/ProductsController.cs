using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QiECommerceAPI.Services;
using QiECommerceAPI.Models;

namespace QiECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProductService productService, ILogger<ProductsController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        // GET: /api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] string? category,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] bool? inStock,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var (items, totalCount) = await _productService.GetPaginatedProductsAsync(
                pageNumber, pageSize, category, minPrice, maxPrice, inStock);

            return Ok(new
            {
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = items
            });
        }

        // POST: /api/products (Admin only)
        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            _logger.LogInformation($"Creating new product: {product.Name}");
            var created = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProducts), new { id = created.Id }, created);
        }

        // PUT: /api/products/{id} (Admin only)
        [HttpPut("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            if (id != updatedProduct.Id)
                return BadRequest("Product ID mismatch.");

            var result = await _productService.UpdateProductAsync(id, updatedProduct);
            if (result == null)
                return NotFound("Product not found.");

            return NoContent();
        }

        // DELETE: /api/products/{id} (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var success = await _productService.DeleteProductAsync(id);
            if (!success)
                return NotFound("Product not found.");

            return NoContent();
        }
    }
}
