using MongoDB.Driver;
using ClothingPOS.API.Models;
using ClothingPOS.API.Data;

namespace ClothingPOS.API.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product?> GetProductAsync(string id);
        Task<Product> CreateProductAsync(Product product);
        Task UpdateProductAsync(string id, Product product);
        Task RemoveProductAsync(string id);
        Task<List<Product>> GetProductsByCategoryAsync(string category);
        Task<List<Product>> SearchProductsAsync(string searchTerm);
    }

    public class ProductService : IProductService
    {
        private readonly IMongoCollection<Product> _products;

        public ProductService(MongoDbContext context)
        {
            _products = context.Products;
        }

        public async Task<List<Product>> GetProductsAsync() =>
            await _products.Find(_ => true).ToListAsync();

        public async Task<Product?> GetProductAsync(string id) =>
            await _products.Find(p => p.Id == id).FirstOrDefaultAsync();

        public async Task<Product> CreateProductAsync(Product product)
        {
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;
            await _products.InsertOneAsync(product);
            return product;
        }

        public async Task UpdateProductAsync(string id, Product product)
        {
            product.UpdatedAt = DateTime.UtcNow;
            await _products.ReplaceOneAsync(p => p.Id == id, product);
        }

        public async Task RemoveProductAsync(string id) =>
            await _products.DeleteOneAsync(p => p.Id == id);

        public async Task<List<Product>> GetProductsByCategoryAsync(string category) =>
            await _products.Find(p => p.Category.ToLower() == category.ToLower()).ToListAsync();

        public async Task<List<Product>> SearchProductsAsync(string searchTerm) =>
            await _products.Find(p => p.Name.ToLower().Contains(searchTerm.ToLower())).ToListAsync();
    }
}
