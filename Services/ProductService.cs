using QiECommerceAPI.Models;
using QiECommerceAPI.Repositories;

namespace QiECommerceAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<(IEnumerable<Product> items, int totalCount)> GetPaginatedProductsAsync(
            int pageNumber,
            int pageSize,
            string? category,
            decimal? minPrice,
            decimal? maxPrice,
            bool? inStock)
        {
            // First, filter
            var filtered = await _productRepository.FilterProductsAsync(category, minPrice, maxPrice, inStock);
            var totalCount = filtered.Count();

            // Then paginate
            var skip = (pageNumber - 1) * pageSize;
            var paged = filtered.Skip(skip).Take(pageSize);

            return (paged, totalCount);
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateProductAsync(int id, Product updatedProduct)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null) return null;

            existing.Name = updatedProduct.Name;
            existing.Description = updatedProduct.Description;
            existing.PriceIQD = updatedProduct.PriceIQD;
            existing.Stock = updatedProduct.Stock;
            existing.Category = updatedProduct.Category;
            existing.SupplierName = updatedProduct.SupplierName;

            _productRepository.Update(existing);
            await _productRepository.SaveChangesAsync();

            return existing;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null) return false;

            _productRepository.Remove(existing);
            await _productRepository.SaveChangesAsync();
            return true;
        }
    }
}
