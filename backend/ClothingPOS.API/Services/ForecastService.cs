using ClothingPOS.API.Models;
using System.Text.Json;

namespace ClothingPOS.API.Services
{
    public class ForecastService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ForecastService> _logger;
        private readonly string _aiApiBaseUrl;

        public ForecastService(HttpClient httpClient, ILogger<ForecastService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _aiApiBaseUrl = configuration.GetValue<string>("AIModel:BaseUrl") ?? "http://localhost:5001";
            
            // Configure HttpClient
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public class SalesForecast
        {
            public string Month { get; set; } = string.Empty;
            public decimal PredictedSales { get; set; }
            public decimal ActualSales { get; set; }
            public string Trend { get; set; } = string.Empty;
            public int? Confidence { get; set; }
        }

        public class AIForecastResponse
        {
            public bool Success { get; set; }
            public List<AIForecastData> Forecast { get; set; } = new();
            public string ModelType { get; set; } = string.Empty;
            public string GeneratedAt { get; set; } = string.Empty;
        }

        public class AIForecastData
        {
            public string Month { get; set; } = string.Empty;
            public decimal PredictedSales { get; set; }
            public decimal ActualSales { get; set; }
            public string Trend { get; set; } = string.Empty;
        }

        public class AICategoryResponse
        {
            public bool Success { get; set; }
            public Dictionary<string, decimal> Categories { get; set; } = new();
            public string GeneratedAt { get; set; } = string.Empty;
        }

        public async Task<List<SalesForecast>> GetSalesForecastAsync(int monthsAhead = 6)
        {
            try
            {
                _logger.LogInformation("Requesting sales forecast for {MonthsAhead} months from AI model", monthsAhead);
                
                var response = await _httpClient.GetAsync($"{_aiApiBaseUrl}/api/forecast/sales?months={monthsAhead}");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var aiResponse = JsonSerializer.Deserialize<AIForecastResponse>(jsonContent, options);

                    if (aiResponse?.Success == true && aiResponse.Forecast != null)
                    {
                        var forecasts = aiResponse.Forecast.Select(f => new SalesForecast
                        {
                            Month = f.Month,
                            PredictedSales = f.PredictedSales,
                            ActualSales = f.ActualSales,
                            Trend = f.Trend
                        }).ToList();

                        _logger.LogInformation("Successfully retrieved {Count} forecast records from AI model ({ModelType})", 
                            forecasts.Count, aiResponse.ModelType);
                        
                        return forecasts;
                    }
                }
                else
                {
                    _logger.LogWarning("AI API returned status code: {StatusCode}", response.StatusCode);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error connecting to AI model API");
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Timeout connecting to AI model API");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing AI model response");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting forecast from AI model");
            }

            // Fallback to simulated data if AI model is unavailable
            _logger.LogInformation("Using fallback forecast data");
            return await GetFallbackForecastAsync(monthsAhead);
        }

        public async Task<Dictionary<string, decimal>> GetCategoryForecastAsync()
        {
            try
            {
                _logger.LogInformation("Requesting category forecast from AI model");
                
                var response = await _httpClient.GetAsync($"{_aiApiBaseUrl}/api/forecast/categories");
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonContent = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var aiResponse = JsonSerializer.Deserialize<AICategoryResponse>(jsonContent, options);

                    if (aiResponse?.Success == true && aiResponse.Categories != null)
                    {
                        _logger.LogInformation("Successfully retrieved category forecast from AI model");
                        return aiResponse.Categories;
                    }
                }
                else
                {
                    _logger.LogWarning("AI API returned status code: {StatusCode} for category forecast", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category forecast from AI model");
            }

            // Fallback data
            _logger.LogInformation("Using fallback category forecast data");
            return new Dictionary<string, decimal>
            {
                { "Men", 3500m },
                { "Women", 4200m },
                { "Unisex", 2800m },
                { "Accessories", 1200m }
            };
        }

        public async Task<List<Product>> GetLowStockProductsAsync(List<Product> products)
        {
            await Task.Delay(50);
            return products.Where(p => p.Stock <= 10).ToList();
        }

        public async Task<bool> CheckAIModelHealthAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_aiApiBaseUrl}/");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private async Task<List<SalesForecast>> GetFallbackForecastAsync(int monthsAhead)
        {
            await Task.Delay(100);
            
            var forecasts = new List<SalesForecast>();
            var baseAmount = 1000m;

            for (int i = 0; i < monthsAhead; i++)
            {
                var month = DateTime.Now.AddMonths(i);
                var seasonalFactor = GetSeasonalFactor(month.Month);
                var growthFactor = 1 + (i * 0.05m); // 5% monthly growth
                
                forecasts.Add(new SalesForecast
                {
                    Month = month.ToString("MMM yyyy"),
                    PredictedSales = baseAmount * seasonalFactor * growthFactor,
                    ActualSales = i == 0 ? baseAmount * seasonalFactor * 0.95m : 0,
                    Trend = i < 2 ? "Growing" : "Stable"
                });
            }

            return forecasts;
        }

        private decimal GetSeasonalFactor(int month)
        {
            return month switch
            {
                12 or 1 or 2 => 1.3m, // Winter boost
                3 or 4 or 5 => 1.1m,  // Spring
                6 or 7 or 8 => 0.9m,  // Summer dip
                9 or 10 or 11 => 1.2m, // Fall/Back to school
                _ => 1.0m
            };
        }
    }
}
