using MongoDB.Driver;
using ClothingPOS.API.Models;
using ClothingPOS.API.Data;

namespace ClothingPOS.API.Services
{
    public interface ISaleService
    {
        Task<List<Sale>> GetSalesAsync();
        Task<Sale?> GetSaleAsync(string id);
        Task<Sale> CreateSaleAsync(Sale sale);
        Task UpdateSaleAsync(string id, Sale sale);
        Task RemoveSaleAsync(string id);
        Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<List<Sale>> GetSalesByProductAsync(string productId);
        Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null);
    }

    public class SaleService : ISaleService
    {
        private readonly IMongoCollection<Sale> _sales;

        public SaleService(MongoDbContext context)
        {
            _sales = context.Sales;
        }

        public async Task<List<Sale>> GetSalesAsync() =>
            await _sales.Find(_ => true).SortByDescending(s => s.SaleDate).ToListAsync();

        public async Task<Sale?> GetSaleAsync(string id) =>
            await _sales.Find(s => s.Id == id).FirstOrDefaultAsync();

        public async Task<Sale> CreateSaleAsync(Sale sale)
        {
            sale.CreatedAt = DateTime.UtcNow;
            sale.SaleDate = DateTime.UtcNow;
            await _sales.InsertOneAsync(sale);
            return sale;
        }

        public async Task UpdateSaleAsync(string id, Sale sale) =>
            await _sales.ReplaceOneAsync(s => s.Id == id, sale);

        public async Task RemoveSaleAsync(string id) =>
            await _sales.DeleteOneAsync(s => s.Id == id);

        public async Task<List<Sale>> GetSalesByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            await _sales.Find(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
                        .SortByDescending(s => s.SaleDate)
                        .ToListAsync();

        public async Task<List<Sale>> GetSalesByProductAsync(string productId) =>
            await _sales.Find(s => s.ProductId == productId)
                        .SortByDescending(s => s.SaleDate)
                        .ToListAsync();

        public async Task<decimal> GetTotalSalesAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var filter = Builders<Sale>.Filter.Empty;
            
            if (startDate.HasValue && endDate.HasValue)
            {
                filter = Builders<Sale>.Filter.Gte(s => s.SaleDate, startDate.Value) &
                        Builders<Sale>.Filter.Lte(s => s.SaleDate, endDate.Value);
            }

            var sales = await _sales.Find(filter).ToListAsync();
            return sales.Sum(s => s.TotalAmount);
        }
    }
}
