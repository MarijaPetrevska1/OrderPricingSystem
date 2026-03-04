using OrderPricingSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register services
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddLogging();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


var dataPath = Path.Combine(app.Environment.ContentRootPath, "Data");
if (!Directory.Exists(dataPath))
{
    Directory.CreateDirectory(dataPath);
}

var productsPath = Path.Combine(dataPath, "products.json");
if (!File.Exists(productsPath))
{
    var defaultProducts = new
    {
        products = new[]
        {
            new { id = "PROD-001", name = "Premium Widget", price = 12.00m }
        }
    };

    var json = System.Text.Json.JsonSerializer.Serialize(defaultProducts, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(productsPath, json);
    Console.WriteLine($"Created products.json at: {productsPath}");
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();