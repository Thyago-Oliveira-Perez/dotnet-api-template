# .NET 8 Production API Template

A clean, scalable and ready-for-production .NET 8 Web API template designed to accelerate backend development with best practices.

## 🚀 Key Features
- 🧱 Clean Architecture (separation of concerns)
- 🔄 RESTful API with structured routing
- 🪵 EF Core with migrations and best practices
- 📈 Health checks, logging (Serilog), OpenAPI/Swagger
- 🐳 Docker support (dev/prod)
- 🔐 Environment config & secrets
- 📦 CI/CD friendly

## 📌 Why this template?
This repo helps teams start backend projects with:
- Focus on maintainability & extensibility
- Minimal boilerplate
- Production-ready defaults

## ⚙️ Tech Stack
- .NET 8
- C#
- Entity Framework Core
- Serilog
- Docker
- Swagger / OpenAPI

## 🏗️ Project Structure

```
src/
├── ApiTemplate.Domain/          # Domain entities and business logic
│   ├── Common/                  # Base entities
│   └── Entities/                # Domain models (Product, etc.)
├── ApiTemplate.Application/     # Application logic and interfaces
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Interfaces/              # Repository and UoW interfaces
│   └── Services/                # Business services
├── ApiTemplate.Infrastructure/  # Data access and external services
│   ├── Data/                    # DbContext and configurations
│   └── Repositories/            # Repository implementations
└── ApiTemplate.API/             # Web API layer
    └── Controllers/             # API endpoints
```

## 🚀 Getting Started

### Prerequisites
- [mise](https://mise.jdx.dev/) - For managing .NET SDK version (or .NET 8 SDK installed manually)
- SQL Server or Docker (for database)

### Using mise (Recommended)

1. **Clone the repository**:
```bash
git clone https://github.com/Thyago-Oliveira-Perez/dotnet-api-template
cd dotnet-api-template
```

2. **Trust and install .NET via mise**:
```bash
mise trust
mise install
```

3. **Update connection string**:
Edit `src/ApiTemplate.API/appsettings.json` and configure your database connection.

4. **Build & Run**:
```bash
dotnet build
dotnet run --project src/ApiTemplate.API/ApiTemplate.API.csproj
```

### Using Docker Compose

1. **Start the application with Docker**:
```bash
docker-compose up --build
```

This will start:
- API on `http://localhost:5000` and `https://localhost:5001`
- SQL Server on `localhost:1433`

2. **Access Swagger UI**:
Open `http://localhost:5000/swagger` in your browser

### Database Migrations

Create and apply migrations using EF Core tools:

```bash
# Install EF Core tools globally
dotnet tool install --global dotnet-ef

# Create a migration
dotnet ef migrations add [migration name] --project src/ApiTemplate.Infrastructure --startup-project src/ApiTemplate.API

# Update the database
dotnet ef database update --project src/ApiTemplate.Infrastructure --startup-project src/ApiTemplate.API
```

## 📊 Architecture Principles

- **Single Responsibility**: Each layer has a distinct purpose
- **Explicit boundaries between layers**: Dependencies flow inward
- **DI for flexibility & testability**: Constructor injection throughout
- **Repository Pattern**: Abstract data access
- **Unit of Work**: Manage transactions

## 🔧 Development

### API Endpoints

- `GET /api/products` - Get all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create a new product
- `PUT /api/products/{id}` - Update a product
- `DELETE /api/products/{id}` - Delete a product
- `GET /health` - Health check endpoint

### Configuration

- `appsettings.json` - Production settings
- `appsettings.Development.json` - Development settings
- Connection strings
- Serilog configuration

### Logging

Logs are written to:
- Console (colored output)
- File (`logs/api-{date}.txt`)

## 🐳 Docker

Build and run with Docker:

```bash
# Build image
docker build -t dotnet-api-template .

# Run container
docker run -p 5000:8080 -p 5001:8081 dotnet-api-template
```

## 🧪 Testing

```bash
dotnet test
```

## 🧪 How to Contribute

PRs and feedback welcome — follow the [CONTRIBUTING.md](./CONTRIBUTING.md) guidelines!

## 📄 License

MIT License
