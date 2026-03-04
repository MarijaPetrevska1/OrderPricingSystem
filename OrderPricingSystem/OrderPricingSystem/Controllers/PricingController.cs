using Microsoft.AspNetCore.Mvc;
using OrderPricingSystem.Models;
using OrderPricingSystem.Services;

namespace OrderPricingSystem.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
    private readonly IPricingService _pricingService;
    private readonly ILogger<PricingController> _logger;

    public PricingController(IPricingService pricingService, ILogger<PricingController> logger)
    {
        _pricingService = pricingService;
        _logger = logger;
    }

    [HttpGet("calculate")]
    public async Task<ActionResult<PricingResponse>> CalculatePrice(
        [FromQuery] string productId,
        [FromQuery] int quantity,
        [FromQuery] string country)
    {
        try
        {
            var request = new OrderRequest
            {
                ProductId = productId,
                Quantity = quantity,
                Country = country
            };

            var result = await _pricingService.CalculatePriceAsync(request);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid request parameters");
            return BadRequest(new { error = ex.Message });
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Product not found");
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error in CalculatePrice");
            return StatusCode(500, new { error = "An unexpected error occurred" });
        }
    }
}
