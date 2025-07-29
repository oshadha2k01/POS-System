using backend.API.Data;
using backend.API.Models;
using MongoDB.Driver;

namespace backend.API.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product?> GetProductAsync(string id);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(string id, Product product);
        Task DeleteProductAsync(string id);
    }

    public class ProductService : IProductService
    {
        private readonly MongoDbContext _context;

        public ProductService(MongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products.Find(_ => true).ToListAsync();
        }

        public async Task<Product?> GetProductAsync(string id)
        {
            return await _context.Products.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Product> CreateProductAsync(Product product)
        {
            await _context.Products.InsertOneAsync(product);
            return product;
        }

        public async Task UpdateProductAsync(string id, Product product)
        {
            await _context.Products.ReplaceOneAsync(x => x.Id == id, product);
        }

        public async Task DeleteProductAsync(string id)
        {
            await _context.Products.DeleteOneAsync(x => x.Id == id);
        }
    }
}
