using Microsoft.AspNetCore.Mvc;
using ClothingPOS.API.Models;
using ClothingPOS.API.Services;

namespace ClothingPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISaleService _saleService;
        private readonly IProductService _productService;
        private readonly ILogger<SalesController> _logger;

        public SalesController(ISaleService saleService, IProductService productService, ILogger<SalesController> logger)
        {
            _saleService = saleService;
            _productService = productService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sale>>> GetSales([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                List<Sale> sales;

                if (startDate.HasValue && endDate.HasValue)
                {
                    sales = await _saleService.GetSalesByDateRangeAsync(startDate.Value, endDate.Value);
                }
                else
                {
                    sales = await _saleService.GetSalesAsync();
                }

                return Ok(sales);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales");
                return StatusCode(500, new { message = "An error occurred while retrieving sales" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Sale>> GetSale(string id)
        {
            try
            {
                var sale = await _saleService.GetSaleAsync(id);
                
                if (sale == null)
                {
                    return NotFound(new { message = $"Sale with ID {id} not found" });
                }
                
                return Ok(sale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sale with ID: {SaleId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the sale" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Sale>> CreateSale(Sale sale)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Get product details to populate sale information
                var product = await _productService.GetProductAsync(sale.ProductId);
                if (product == null)
                {
                    return BadRequest(new { message = "Product not found" });
                }

                // Check stock availability
                if (product.Stock < sale.Quantity)
                {
                    return BadRequest(new { message = "Insufficient stock available" });
                }

                // Populate sale details from product
                sale.ProductName = product.Name;
                sale.ProductCategory = product.Category;
                sale.UnitPrice = product.Price;
                sale.TotalAmount = sale.Quantity * product.Price;

                var createdSale = await _saleService.CreateSaleAsync(sale);

                // Update product stock
                product.Stock -= sale.Quantity;
                await _productService.UpdateProductAsync(product.Id!, product);

                return CreatedAtAction(nameof(GetSale), new { id = createdSale.Id }, createdSale);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating sale");
                return StatusCode(500, new { message = "An error occurred while creating the sale" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSale(string id, Sale sale)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingSale = await _saleService.GetSaleAsync(id);
                if (existingSale == null)
                {
                    return NotFound(new { message = $"Sale with ID {id} not found" });
                }

                sale.Id = id;
                await _saleService.UpdateSaleAsync(id, sale);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating sale with ID: {SaleId}", id);
                return StatusCode(500, new { message = "An error occurred while updating the sale" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSale(string id)
        {
            try
            {
                var sale = await _saleService.GetSaleAsync(id);
                if (sale == null)
                {
                    return NotFound(new { message = $"Sale with ID {id} not found" });
                }

                await _saleService.RemoveSaleAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting sale with ID: {SaleId}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the sale" });
            }
        }

        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetSalesStats([FromQuery] DateTime? startDate = null, [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var totalSales = await _saleService.GetTotalSalesAsync(startDate, endDate);
                var sales = startDate.HasValue && endDate.HasValue 
                    ? await _saleService.GetSalesByDateRangeAsync(startDate.Value, endDate.Value)
                    : await _saleService.GetSalesAsync();

                var stats = new
                {
                    TotalSales = totalSales,
                    TotalTransactions = sales.Count,
                    AverageTransaction = sales.Count > 0 ? totalSales / sales.Count : 0,
                    TopProducts = sales.GroupBy(s => new { s.ProductId, s.ProductName })
                                      .Select(g => new { ProductName = g.Key.ProductName, Quantity = g.Sum(s => s.Quantity) })
                                      .OrderByDescending(x => x.Quantity)
                                      .Take(5)
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving sales statistics");
                return StatusCode(500, new { message = "An error occurred while retrieving sales statistics" });
            }
        }
    }
}
