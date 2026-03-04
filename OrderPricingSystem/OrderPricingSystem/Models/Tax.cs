namespace OrderPricingSystem.Models;

public class Tax
{
    public string Country { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}