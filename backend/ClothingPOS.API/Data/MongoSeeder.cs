using ClothingPOS.API.Models;
using ClothingPOS.API.Services;

namespace ClothingPOS.API.Data
{
    public class MongoSeeder
    {
        private readonly IProductService _productService;
        private readonly ISaleService _saleService;

        public MongoSeeder(IProductService productService, ISaleService saleService)
        {
            _productService = productService;
            _saleService = saleService;
        }

        public async Task SeedAsync()
        {
            // Check if data already exists
            var existingProducts = await _productService.GetProductsAsync();
            if (existingProducts.Any())
            {
                return; // Data already seeded
            }

            // Seed Products
            var products = new List<Product>
            {
                new Product
                {
                    Name = "Classic White T-Shirt",
                    Price = 19.99m,
                    Stock = 50,
                    Category = "T-Shirts",
                    ImageUrl = "https://example.com/white-tshirt.jpg"
                },
                new Product
                {
                    Name = "Blue Denim Jeans",
                    Price = 79.99m,
                    Stock = 30,
                    Category = "Jeans",
                    ImageUrl = "https://example.com/blue-jeans.jpg"
                },
                new Product
                {
                    Name = "Red Summer Dress",
                    Price = 59.99m,
                    Stock = 25,
                    Category = "Dresses",
                    ImageUrl = "https://example.com/red-dress.jpg"
                },
                new Product
                {
                    Name = "Black Leather Jacket",
                    Price = 149.99m,
                    Stock = 15,
                    Category = "Jackets",
                    ImageUrl = "https://example.com/leather-jacket.jpg"
                },
                new Product
                {
                    Name = "Cotton Polo Shirt",
                    Price = 34.99m,
                    Stock = 40,
                    Category = "Polo Shirts",
                    ImageUrl = "https://example.com/polo-shirt.jpg"
                },
                new Product
                {
                    Name = "Khaki Chinos",
                    Price = 49.99m,
                    Stock = 35,
                    Category = "Pants",
                    ImageUrl = "https://example.com/khaki-chinos.jpg"
                },
                new Product
                {
                    Name = "Striped Long Sleeve Shirt",
                    Price = 39.99m,
                    Stock = 28,
                    Category = "Shirts",
                    ImageUrl = "https://example.com/striped-shirt.jpg"
                },
                new Product
                {
                    Name = "Wool Winter Coat",
                    Price = 199.99m,
                    Stock = 12,
                    Category = "Coats",
                    ImageUrl = "https://example.com/winter-coat.jpg"
                }
            };

            foreach (var product in products)
            {
                await _productService.CreateProductAsync(product);
            }

            // Seed some sample sales
            var allProducts = await _productService.GetProductsAsync();
            var sampleSales = new List<Sale>
            {
                new Sale
                {
                    ProductId = allProducts[0].Id!,
                    ProductName = allProducts[0].Name,
                    ProductCategory = allProducts[0].Category,
                    Quantity = 2,
                    UnitPrice = allProducts[0].Price,
                    TotalAmount = 2 * allProducts[0].Price,
                    CustomerName = "John Doe",
                    PaymentMethod = "Cash",
                    SaleDate = DateTime.UtcNow.AddDays(-5)
                },
                new Sale
                {
                    ProductId = allProducts[1].Id!,
                    ProductName = allProducts[1].Name,
                    ProductCategory = allProducts[1].Category,
                    Quantity = 1,
                    UnitPrice = allProducts[1].Price,
                    TotalAmount = allProducts[1].Price,
                    CustomerName = "Jane Smith",
                    PaymentMethod = "Credit Card",
                    SaleDate = DateTime.UtcNow.AddDays(-3)
                },
                new Sale
                {
                    ProductId = allProducts[2].Id!,
                    ProductName = allProducts[2].Name,
                    ProductCategory = allProducts[2].Category,
                    Quantity = 1,
                    UnitPrice = allProducts[2].Price,
                    TotalAmount = allProducts[2].Price,
                    CustomerName = "Bob Johnson",
                    PaymentMethod = "Debit Card",
                    SaleDate = DateTime.UtcNow.AddDays(-1)
                }
            };

            foreach (var sale in sampleSales)
            {
                await _saleService.CreateSaleAsync(sale);
            }
        }
    }
}
