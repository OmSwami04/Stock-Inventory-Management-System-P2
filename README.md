# Stock / Inventory Management System

A production-grade ASP.NET Core 10 Web API built using Clean Architecture and Enterprise Design Patterns, fully aligned with your specific database schema.

## 🏗 Architecture & Implementation

### Architectural Style
This project is implemented as a **Clean Architecture Monolith** (Modular Monolith). It is designed to be easily decomposable into microservices.

### Implementation Layers
- **API Layer**: Handles HTTP requests, Swagger UI, and Global Exception Handling.
- **Application Layer**: Business logic via CQRS (MediatR), DTOs, Mapping, and Validation (FluentValidation).
- **Domain Layer**: Core entities aligned with your specified MySQL schema (`Product`, `StockLevel`, `Supplier`, etc.).
- **Infrastructure Layer**: Concrete implementations for Data Access (EF Core/MySQL), Caching (Redis), Authentication (JWT/BCrypt), and specialized patterns.
- **Interfaces Layer**: Abstractions to decouple business logic from infrastructure.
- **Shared Layer**: Cross-cutting concerns like custom exceptions.

## ✨ Implemented Patterns & Functionalities

### Design Patterns
1. **CQRS (Command Query Responsibility Segregation)**: Commands for writes, Queries for reads.
2. **Repository Pattern**: Abstracted data access via Generic and Specific repositories.
3. **Unit of Work Pattern**: Atomic transactions across multiple repositories.
4. **Factory Design Pattern**: `InventoryValuationFactory` for dynamic method selection (FIFO, LIFO, Weighted Average).
5. **Chain of Responsibility Pattern**: `StockTransactionValidationPipeline` for multi-step transaction verification.
6. **Singleton Pattern**: Used for Caching and Analytics services.

### Updated Data Schema
The system now reflects the following entities:
- **Product**: Aligned with new fields (`SKU`, `ProductName`, `CategoryId`, `UnitOfMeasure`, `Cost`, `ListPrice`, `IsActive`).
- **ProductCategory**: Supporting hierarchical categories.
- **Warehouse**: Updated properties (`WarehouseName`, `Location`, `Capacity`).
- **StockLevel**: (Renamed from Stock) Tracking quantity in specific warehouses with reorder and safety stock levels.
- **StockTransaction**: Detailed movement logs with `TransactionType` and `Reference`.
- **Supplier & ProductSupplier**: For managing external vendor relationships.

## 🚀 Getting Started

### Prerequisites
- .NET 10 SDK
- MySQL Server
- Redis (Optional)

### Running the Project
1. **Update Configuration**: Edit `src/Api/appsettings.json` with your MySQL connection string.
2. **Apply Migrations**: (If starting fresh)
   ```powershell
   dotnet ef database update --project src/Infrastructure --startup-project src/Api
   ```
3. **Run the API**:
   ```powershell
   dotnet run --project src/Api
   ```
4. **Swagger UI**: Access documentation at `http://localhost:5153/swagger`.

## ✅ Final Verification
- **Build Status**: 100% Successful on .NET 10.
- **Schema Alignment**: All entities, repositories, and logic fully updated to match the provided structure.
