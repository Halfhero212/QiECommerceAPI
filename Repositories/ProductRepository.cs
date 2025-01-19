using Microsoft.EntityFrameworkCore;
using QiECommerceAPI.Data;
using QiECommerceAPI.Models;

namespace QiECommerceAPI.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _dbContext.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _dbContext.Products.CountAsync();
        }

        public async Task<IEnumerable<Product>> GetPagedAsync(int skip, int take)
        {
            return await _dbContext.Products.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<IEnumerable<Product>> FilterProductsAsync(string? category, decimal? minPrice, decimal? maxPrice, bool? inStock)
        {
            var query = _dbContext.Products.AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category == category);

            if (minPrice.HasValue)
                query = query.Where(p => p.PriceIQD >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.PriceIQD <= maxPrice.Value);

            if (inStock.HasValue && inStock.Value)
                query = query.Where(p => p.Stock > 0);

            return await query.ToListAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _dbContext.Products.AddAsync(product);
        }

        public void Update(Product product)
        {
            _dbContext.Products.Update(product);
        }

        public void Remove(Product product)
        {
            _dbContext.Products.Remove(product);
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
