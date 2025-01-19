using QiECommerceAPI.Models;

namespace QiECommerceAPI.Services
{
    public interface IProductService
    {
        Task<(IEnumerable<Product> items, int totalCount)> GetPaginatedProductsAsync(
            int pageNumber,
            int pageSize,
            string? category,
            decimal? minPrice,
            decimal? maxPrice,
            bool? inStock);

        Task<Product> CreateProductAsync(Product product);
        Task<Product?> UpdateProductAsync(int id, Product updatedProduct);
        Task<bool> DeleteProductAsync(int id);
    }
}
