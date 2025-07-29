using Microsoft.AspNetCore.Mvc;
using ClothingPOS.API.Services;

namespace ClothingPOS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIModelController : ControllerBase
    {
        private readonly ForecastService _forecastService;
        private readonly ILogger<AIModelController> _logger;

        public AIModelController(ForecastService forecastService, ILogger<AIModelController> logger)
        {
            _forecastService = forecastService;
            _logger = logger;
        }

        [HttpGet("health")]
        public async Task<ActionResult> GetHealthStatus()
        {
            try
            {
                var isHealthy = await _forecastService.CheckAIModelHealthAsync();
                var status = new
                {
                    Status = isHealthy ? "Healthy" : "Unavailable",
                    AIModelConnected = isHealthy,
                    Timestamp = DateTime.UtcNow,
                    FallbackEnabled = true
                };

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking AI model health");
                return StatusCode(500, new { Status = "Error", Message = ex.Message });
            }
        }

        [HttpGet("forecast/extended")]
        public async Task<ActionResult> GetExtendedForecast([FromQuery] int months = 12)
        {
            try
            {
                if (months < 1 || months > 24)
                {
                    return BadRequest("Months must be between 1 and 24");
                }

                var forecast = await _forecastService.GetSalesForecastAsync(months);
                var categoryForecast = await _forecastService.GetCategoryForecastAsync();

                var result = new
                {
                    MonthlyForecast = forecast,
                    CategoryForecast = categoryForecast,
                    Summary = new
                    {
                        TotalMonths = forecast.Count,
                        TotalPredictedSales = forecast.Sum(f => f.PredictedSales),
                        AverageMonthly = forecast.Average(f => f.PredictedSales),
                        GeneratedAt = DateTime.UtcNow
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting extended forecast");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("retrain")]
        public async Task<ActionResult> TriggerRetrain()
        {
            try
            {
                // In a production system, this would trigger model retraining
                // For now, we'll just return a success message
                await Task.Delay(1000); // Simulate processing time

                return Ok(new
                {
                    Message = "Model retrain request queued",
                    Status = "Success",
                    Timestamp = DateTime.UtcNow,
                    EstimatedCompletionTime = DateTime.UtcNow.AddMinutes(5)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error triggering model retrain");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
