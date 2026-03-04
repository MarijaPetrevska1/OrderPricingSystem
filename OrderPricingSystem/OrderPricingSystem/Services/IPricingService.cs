using OrderPricingSystem.Models;

namespace OrderPricingSystem.Services;

public interface IPricingService
{
    Task<PricingResponse> CalculatePriceAsync(OrderRequest request);
}
