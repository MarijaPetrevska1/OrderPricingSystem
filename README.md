### 💻Order Pricing System for an e-commerce platform

OrderPricingSystem is a .NET Web API that calculates order pricing for an e-commerce platform.

The system processes product pricing based on quantity, applies tiered discounts when applicable, calculates country-based tax, and returns a detailed pricing breakdown.

### Technologies Used: 
.NET (latest stable version), ASP.NET Core Web API, JSON file as data source (no database), Dependency Injection, Clean OOP structure

### 🚀 How to run this project: 

1. Clone the repository
git clone [<your-repository-link>](https://github.com/MarijaPetrevska1/OrderPricingSystem)
cd OrderPricingSystem

2. Run the application

dotnet run https://localhost:XXXX

3. Open Swagger UI
 
https://localhost:XXXX/swagger

Use the endpoint: GET /api/pricing/calculate

Example request: /api/pricing/calculate?productId=PROD-001&quantity=55&country=MK

### Project Structure

- #### Models

OrderRequest, PricingResponse, Product, ProductsContainer, Discount, Tax.

These correctly represent the  request, response, and supporting objects.

- #### Services

*IPricingService* interface 

*PricingService* implementation 

Loads products from *products.json* 

Calculates subtotal, discount (tiered + threshold), tax 

Builds response 

Logging + error handling included 

Note: Discount and tax calculations are correct, and rounding is applied to 2 decimals.

- #### Controller

*PricingController* 

GET /api/pricing/calculate endpoint 

Validates query parameters, handles exceptions 

Returns BadRequest, NotFound, or 500 as needed 

- #### Program.cs

Registers services and logging 

Configures Swagger 

Creates Data folder and default products.json if missing 

Maps controllers 

This ensures the project runs immediately after cloning and dotnet run.

- #### products.json

Default product included 

{
  "products": [
    {
      "id": "PROD-001",
      "name": "Premium Widget",
      "price": 12.00
    }
  ]
}

### 🧮 Calculated Results for Test Cases

Product: PROD-001 – Premium Widget
Unit Price: 12.00 EUR

### Test Case 1

*Input:*

productId=PROD-001
quantity=55
country=MK

*Calculation:*

Subtotal = 55 × 12 = 660

Discount = 10% (because quantity ≥ 50 and subtotal ≥ 500)

Discount amount = 66

Subtotal after discount = 594

Tax (18% MK) = 106.92

✅ Final Price = 700.92 EUR

### Test Case 2

*Input:*

productId=PROD-001
quantity=100
country=DE

*Calculation:*

Subtotal = 100 × 12 = 1200

Discount = 15%

Discount amount = 180

Subtotal after discount = 1020

Tax (20% DE) = 204

✅ Final Price = 1224.00 EUR

### Test Case 3

*Input:*

productId=PROD-001
quantity=25
country=USA

*Calculation:*

Subtotal = 25 × 12 = 300

No discount (subtotal < 500 threshold)

Tax (10% USA) = 30

✅ Final Price = 330.00 EUR

### 🛠 Bugs Fixed

1️⃣ Incorrect Tax Calculation

*Bug:*

Tax was calculated on the original subtotal.

decimal taxAmount = subtotal * taxRate;

*Fix:*

Tax is now calculated on the amount AFTER discount:

decimal taxAmount = subtotalAfterDiscount * taxRate;

2️⃣ Incorrect Discount Threshold

*Bug:*

Discount applied only when subtotal > 500.

if (subtotal > 500)

*Fix:*

Changed to match business rule:

if (subtotal >= 500)

3️⃣ Incorrect Tier Logic

*Bug:*

Discount conditions overwrote each other and were not properly ordered.

if (quantity >= 10)
    discount = 0.05m;
if (quantity > 50)
    discount = 0.10m;
if (quantity >= 100)
    discount = 0.15m;

*Problem:*

Conditions overlapped

Order was incorrect

Could cause wrong discount

*Fix:*

Implemented proper tier-based logic:
if (quantity >= 100)
    return 0.15m;
if (quantity >= 50)
    return 0.10m;
if (quantity >= 10)
    return 0.05m;

4️⃣ Missing Product Loading

Implemented GetProduct() method to load products from products.json.

5️⃣ Missing Response Builder

Implemented BuildResponse() method to return the full pricing breakdown.

### 🧱 OOP Principles Applied


- Separation of concerns (Controller vs Service)

- Dependency Injection

- Logging

- Error handling with proper HTTP status codes

- Encapsulation (private helper methods)

- Clean tier-based discount logic

