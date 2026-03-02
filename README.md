## Order Pricing System for an e-commerce platform

OrderPricingSystem is a .NET Web API that calculates order pricing for an e-commerce platform.
The system processes product pricing based on quantity, applies tiered discounts when applicable, calculates country-based tax, and returns a detailed pricing breakdown.

Technologies Used: .NET (latest stable version), ASP.NET Core Web API, JSON file as data source (no database), Dependency Injection, Clean OOP structure

🚀 How to Run

1. Clone the repository

2. Navigate to the project folder

dotnet run

3. Open your browser or Postman and call:

GET /api/pricing/calculate?productId=PROD-001&quantity=55&country=MK
