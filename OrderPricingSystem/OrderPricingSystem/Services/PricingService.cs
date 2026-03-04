using System.Text.Json;
using OrderPricingSystem.Models;

namespace OrderPricingSystem.Services;

public class PricingService : IPricingService
{
    private readonly ILogger<PricingService> _logger;
    private List<Product> _products;

    public PricingService(ILogger<PricingService> logger)
    {
        _logger = logger;
        _products = LoadProducts();
    }

    private List<Product> LoadProducts()
    {
        try
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var projectPath = Directory.GetCurrentDirectory();

            string[] possiblePaths = new[]
            {
                Path.Combine(projectPath, "Data", "products.json"),
                Path.Combine(basePath, "Data", "products.json"),
                Path.Combine(projectPath, "products.json"),
                "products.json"
            };

            foreach (var path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    _logger.LogInformation($"Loading products from: {path}");
                    var json = File.ReadAllText(path);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var container = JsonSerializer.Deserialize<ProductsContainer>(json, options);

                    if (container?.Products != null && container.Products.Any())
                    {
                        _logger.LogInformation($"Successfully loaded {container.Products.Count} products");
                        return container.Products;
                    }
                }
            }

            throw new FileNotFoundException("products.json not found. Please ensure the file exists in the Data folder.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading products from JSON file");
            throw;
        }
    }

    public async Task<PricingResponse> CalculatePriceAsync(OrderRequest request)
    {
        try
        {
            _logger.LogInformation("CalculatePriceAsync called with ProductId: {ProductId}, Quantity: {Quantity}, Country: {Country}",
                request.ProductId, request.Quantity, request.Country);

            // Validate request
            if (string.IsNullOrEmpty(request.ProductId))
                throw new ArgumentException("ProductId is required");

            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0");

            if (string.IsNullOrEmpty(request.Country))
                throw new ArgumentException("Country is required");

            var product = GetProduct(request.ProductId);
            if (product == null)
            {
                _logger.LogWarning($"Product with ID {request.ProductId} not found. Available IDs: {string.Join(", ", _products.Select(p => p.Id))}");
                throw new KeyNotFoundException($"Product with ID {request.ProductId} not found");
            }

            // Calculate subtotal
            decimal subtotal = request.Quantity * product.Price;

            // Calculate discount
            var (discountPct, discountAmount, subtotalAfterDiscount) = CalculateDiscount(request.Quantity, subtotal);

            // Calculate tax
            var (taxRate, taxAmount) = CalculateTax(request.Country, subtotalAfterDiscount);

            // Calculate final price
            decimal finalPrice = subtotalAfterDiscount + taxAmount;

            // Build response
            var response = BuildResponse(product, request, subtotal, discountPct, discountAmount,
                subtotalAfterDiscount, taxRate, taxAmount, finalPrice);

            _logger.LogInformation("Price calculation completed. Final Price: {FinalPrice}", finalPrice);

            return await Task.FromResult(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating price for ProductId: {ProductId}", request.ProductId);
            throw;
        }
    }

    private Product GetProduct(string productId)
    {
        return _products.FirstOrDefault(p => p.Id.Equals(productId, StringComparison.OrdinalIgnoreCase));
    }

    private (decimal discountPct, decimal discountAmount, decimal subtotalAfterDiscount) CalculateDiscount(int quantity, decimal subtotal)
    {
        decimal discountPct = 0;

        if (subtotal >= 500)
        {
            if (quantity >= 100)
            {
                discountPct = 0.15m;
            }
            else if (quantity >= 50)
            {
                discountPct = 0.10m;
            }
            else if (quantity >= 10)
            {
                discountPct = 0.05m;
            }
        }

        decimal discountAmount = subtotal * discountPct;
        decimal subtotalAfterDiscount = subtotal - discountAmount;

        return (discountPct, discountAmount, subtotalAfterDiscount);
    }

    private (decimal taxRate, decimal taxAmount) CalculateTax(string country, decimal amount)
    {
        decimal taxRate = GetTaxRate(country);
        decimal taxAmount = amount * taxRate;

        return (taxRate, taxAmount);
    }

    private decimal GetTaxRate(string country)
    {
        return country?.ToUpperInvariant() switch
        {
            "MK" => 0.18m,
            "DE" => 0.20m,
            "FR" => 0.20m,
            "USA" => 0.10m,
            _ => throw new ArgumentException($"Unsupported country: {country}")
        };
    }

    private PricingResponse BuildResponse(
        Product product,
        OrderRequest request,
        decimal subtotal,
        decimal discountPct,
        decimal discountAmount,
        decimal subtotalAfterDiscount,
        decimal taxRate,
        decimal taxAmount,
        decimal finalPrice)
    {
        return new PricingResponse
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Quantity = request.Quantity,
            UnitPrice = product.Price,
            Country = request.Country,
            Subtotal = Math.Round(subtotal, 2),
            Discount = new Discount
            {
                Percentage = discountPct,
                Amount = Math.Round(discountAmount, 2)
            },
            SubtotalAfterDiscount = Math.Round(subtotalAfterDiscount, 2),
            Tax = new Tax
            {
                Country = request.Country,
                Rate = taxRate,
                Amount = Math.Round(taxAmount, 2)
            },
            FinalPrice = Math.Round(finalPrice, 2)
        };
    }
}