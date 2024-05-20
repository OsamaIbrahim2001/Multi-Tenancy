
using Microsoft.EntityFrameworkCore;
using MultiTenancy.Data;

namespace MultiTenancy.Services
{
    public class ProductService(AppDataContext context) : IProductService
    {
        private readonly AppDataContext _context = context;

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync() => await _context.Products.ToListAsync();

        public async Task<Product?> GetByIdAsync(int id) => await _context.Products.FindAsync(id);
    }
}
