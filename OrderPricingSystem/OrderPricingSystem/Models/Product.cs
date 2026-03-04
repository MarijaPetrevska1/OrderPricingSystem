namespace OrderPricingSystem.Models;

public class Product
{
    public string Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public class ProductsContainer
{
    public List<Product> Products { get; set; }
}
