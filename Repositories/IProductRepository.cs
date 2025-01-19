using QiECommerceAPI.Models;

namespace QiECommerceAPI.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<int> GetCountAsync();
        Task<IEnumerable<Product>> GetPagedAsync(int skip, int take);
        Task<IEnumerable<Product>> FilterProductsAsync(string? category, decimal? minPrice, decimal? maxPrice, bool? inStock);

        Task AddAsync(Product product);
        void Update(Product product);
        void Remove(Product product);

        Task SaveChangesAsync();
    }
}
