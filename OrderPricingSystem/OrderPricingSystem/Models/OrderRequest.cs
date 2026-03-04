namespace OrderPricingSystem.Models
{
    public class OrderRequest
    {
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public string Country { get; set; } // "MK", "DE", "FR", "USA"
    }
}
