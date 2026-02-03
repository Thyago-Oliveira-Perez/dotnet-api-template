# .NET 8 Production-Ready API Template

A clean, scalable, and production-ready .NET 8 Web API template following Clean Architecture principles with built-in observability and code quality tools.

## ⚡ Quick Start

Get started in under 5 minutes:

```bash
# Clone the repository
git clone https://github.com/Thyago-Oliveira-Perez/dotnet-api-template
cd dotnet-api-template

# Option 1: Run with Docker (recommended - includes observability stack)
docker-compose up -d

# Option 2: Run locally (requires .NET 8)
mise trust && mise install  # Or install .NET 8 manually
dotnet run --project src/ApiTemplate.API
```

**Access the application:**
- API: http://localhost:5000/swagger
- Kibana (logs): http://localhost:5601
- Health: http://localhost:5000/health

📖 **New to this template?** See the [Quick Start Guide](QUICKSTART.md) for detailed setup instructions.

## 🎯 What's Included

### Core Features
- ✅ **Clean Architecture** - Domain, Application, Infrastructure, API layers
- ✅ **Entity Framework Core 8** - With migrations and PostgreSQL support
- ✅ **Repository & Unit of Work** - Clean data access patterns
- ✅ **RESTful API** - Full CRUD example with Products controller
- ✅ **Swagger/OpenAPI** - Interactive API documentation
- ✅ **Health Checks** - Monitor application and database health
- ✅ **Docker Support** - Multi-stage builds for dev and production
- ✅ **Testcontainers** - Real database integration tests for CI/CD

### Observability Stack
- 📊 **Elasticsearch** - Centralized log storage and search
- 📈 **Kibana** - Log visualization and analysis
- 🔍 **Elastic APM** - Application performance monitoring
- 🪵 **Serilog** - Structured logging with ECS format
- 📉 **Distributed Tracing** - Track requests across the application

### Code Quality Tools
- 🔬 **SonarAnalyzer.CSharp** - Bug detection and security analysis
- 📐 **Meziantou.Analyzer** - Best practices and performance suggestions
- 💡 **Roslynator.Analyzers** - 500+ code analysis rules
- ✨ **EditorConfig** - Consistent code style enforcement

### DevOps Ready
- 🐳 **Docker Compose** - Full stack with single command
- 🔄 **GitHub Actions** - CI/CD pipeline with Docker support
- 🔧 **mise config** - Version management for .NET SDK
- 🧪 **Integration Tests** - End-to-end API tests with Testcontainers (17 tests)
- 🎯 **Domain Tests** - Unit tests for business logic (7 tests)

## 📌 Why This Template?

Skip the boilerplate and start with:
- Production-ready architecture
- Enterprise-grade observability
- Automated code quality checks
- Complete working example

## 🛠️ Tech Stack

- **.NET 8** - Latest LTS version
- **Entity Framework Core** - ORM with migrations
- **PostgreSQL 16** - Lightweight, fast, open-source database
- **Testcontainers** - Real database integration tests
- **Serilog** - Structured logging with ECS format
- **Elasticsearch** - Log storage and search
- **Kibana** - Log visualization and dashboards
- **Elastic APM** - Performance monitoring
- **Docker & Docker Compose** - Containerization
- **Swagger/OpenAPI** - API documentation
- **xUnit & FluentAssertions** - Testing framework
- **Code Analyzers** - SonarAnalyzer, Meziantou, Roslynator

## 📁 Project Structure

```
src/
├── ApiTemplate.Domain/          # Domain entities and business rules
│   ├── Common/                  # Base entities with audit fields
│   └── Entities/                # Domain models (Product, etc.)
│
├── ApiTemplate.Application/     # Application logic and interfaces
│   ├── DTOs/                    # Data Transfer Objects
│   ├── Interfaces/              # Repository and service contracts
│   └── Services/                # Business logic implementation
│
├── ApiTemplate.Infrastructure/  # Data access and external services
│   ├── Data/                    # DbContext and EF configurations
│   └── Repositories/            # Repository implementations
│
└── ApiTemplate.API/             # Web API and presentation layer
    └── Controllers/             # REST API endpoints

tests/
└── ApiTemplate.Tests/           # Unit tests for all layers
```

📖 **Learn more:** [Architecture Documentation](ARCHITECTURE.md)

## 🚀 Getting Started

### Prerequisites

**Option 1: With mise (Recommended)**
- [mise](https://mise.jdx.dev/) for .NET version management
- Docker and Docker Compose (for full stack)

**Option 2: Manual**
- .NET 8 SDK
- PostgreSQL (or Docker)

### Installation

1. **Clone and setup**
```bash
git clone https://github.com/Thyago-Oliveira-Perez/dotnet-api-template
cd dotnet-api-template
mise trust && mise install  # Or ensure .NET 8 is installed
```

2. **Configure database**
Edit `src/ApiTemplate.API/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=apitemplatedb;Username=postgres;Password=postgres"
  }
}
```

3. **Run migrations**
```bash
dotnet ef database update --project src/ApiTemplate.Infrastructure --startup-project src/ApiTemplate.API
```

4. **Start the application**
```bash
# Local development
dotnet run --project src/ApiTemplate.API

# Or with Docker (includes Elasticsearch, Kibana, APM)
docker-compose up -d
```

### Verify Installation

```bash
# Check health
curl http://localhost:5000/health

# Try the API
curl http://localhost:5000/api/products

# Open Swagger UI
open http://localhost:5000/swagger

# View logs in Kibana (if using Docker)
open http://localhost:5601
```

### Using Docker Compose

**Start everything with one command:**

```bash
docker-compose up -d
```

This starts:
- **API**: http://localhost:5000 (Swagger: /swagger)
- **PostgreSQL**: localhost:5432
- **Elasticsearch**: localhost:9200
- **Kibana**: http://localhost:5601
- **APM Server**: localhost:8200

**Verify it's working:**

```bash
# Check health
curl http://localhost:5000/health

# Try the API
curl http://localhost:5000/api/products

# Open Swagger
open http://localhost:5000/swagger
```

Services take ~2 minutes to fully initialize.

## 📊 Observability

### Built-in Monitoring

The template includes enterprise-grade observability out of the box:

**Logging**
- Structured logs in ECS (Elastic Common Schema) format
- Automatic log shipping to Elasticsearch
- Console and file output for local development
- Correlation IDs for request tracking

**Application Performance Monitoring (APM)**
- Request/response tracking
- Database query performance
- Error rates and stack traces
- Response time percentiles (p95, p99)
- Distributed tracing

### Using Kibana

After starting with Docker Compose:

1. **View Logs**
   - Open http://localhost:5601
   - Go to **Discover**
   - View real-time logs with filtering

2. **Monitor Performance**
   - Go to **Observability → APM**
   - View service overview, transactions, and errors

3. **Create Dashboards**
   - Go to **Dashboard → Create dashboard**
   - Add visualizations for custom metrics

### Configuration

**appsettings.json:**
```json
{
  "Elasticsearch": {
    "Uri": "http://localhost:9200",
    "IndexFormat": "apitemplate-logs-{0:yyyy.MM.dd}"
  },
  "ElasticApm": {
    "ServiceName": "ApiTemplate",
    "ServerUrl": "http://localhost:8200",
    "Environment": "Development"
  }
}
```

**Disable observability for local development:**
```bash
# Run without Docker - logs to console/file only
dotnet run --project src/ApiTemplate.API
```

## 🔬 Code Quality

### Built-in Analyzers

Three powerful analyzers run automatically during build:

1. **SonarAnalyzer.CSharp**
   - Detects bugs and code smells
   - Security vulnerability detection
   - Cognitive complexity analysis

2. **Meziantou.Analyzer**
   - Performance best practices
   - Modern C# pattern suggestions
   - API usage recommendations

3. **Roslynator.Analyzers**
   - 500+ code quality rules
   - Code style enforcement
   - Refactoring suggestions

### Configuration

Rules are configured in `.editorconfig`:

```ini
# Example: Adjust severity levels
dotnet_diagnostic.S125.severity = warning    # Remove commented code
dotnet_diagnostic.MA0004.severity = warning   # Use ConfigureAwait
dotnet_diagnostic.RCS1036.severity = suggestion  # Remove empty lines
```

### Usage

```bash
# Analyzers run automatically on build
dotnet build

# View all warnings
dotnet build | grep warning

# Treat warnings as errors in CI
dotnet build -warnaserror
```

💡 **Tip:** Your IDE shows analyzer suggestions in real-time as you code!

## 🧪 Testing

The template includes both unit tests and end-to-end integration tests:

```bash
# Run all tests (requires Docker for Testcontainers)
dotnet test

# With coverage
dotnet test /p:CollectCoverage=true

# Run specific test
dotnet test --filter "FullyQualifiedName~Product"
```

**Test Strategy:**
- ✅ **Domain Tests (7 tests):** Entity validation and business rules
- ✅ **Integration Tests (10 tests):** End-to-end API tests with real PostgreSQL database
  - Uses Testcontainers to spin up postgres:16-alpine container
  - Tests complete HTTP request/response cycles
  - Validates database persistence and queries
  - Runs in ~17 seconds, suitable for CI/CD pipelines

**Requirements:**
- Docker must be running for integration tests (Testcontainers)
- GitHub Actions runners have Docker pre-installed

## 🐳 Docker

### Development

```bash
# Start full stack (API + SQL + Elasticsearch + Kibana + APM)
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop all services
docker-compose down
```

### Production

Build optimized image:

```bash
docker build -t dotnet-api-template:latest .
docker run -p 8080:8080 dotnet-api-template:latest
```

### Services

| Service | Port | Purpose |
|---------|------|---------|
| API | 5000, 5001 | Web API |
| PostgreSQL | 5432 | Database |
| Elasticsearch | 9200 | Log storage |
| Kibana | 5601 | Log visualization |
| APM Server | 8200 | Performance monitoring |

## 🔧 Development Workflow

### Making Changes

1. **Update domain models** in `ApiTemplate.Domain/Entities`
2. **Add business logic** in `ApiTemplate.Application/Services`
3. **Implement data access** in `ApiTemplate.Infrastructure/Repositories`
4. **Create API endpoints** in `ApiTemplate.API/Controllers`
5. **Write tests** in `ApiTemplate.Tests`

### Database Migrations

```bash
# Create migration
dotnet ef migrations add <MigrationName> \
  --project src/ApiTemplate.Infrastructure \
  --startup-project src/ApiTemplate.API

# Apply migration
dotnet ef database update \
  --project src/ApiTemplate.Infrastructure \
  --startup-project src/ApiTemplate.API

# Rollback
dotnet ef database update <PreviousMigration> \
  --project src/ApiTemplate.Infrastructure \
  --startup-project src/ApiTemplate.API
```

### API Endpoints

Example endpoints included:

- `GET /api/products` - List all products
- `GET /api/products/{id}` - Get product by ID
- `POST /api/products` - Create new product
- `PUT /api/products/{id}` - Update product
- `DELETE /api/products/{id}` - Delete product
- `GET /health` - Health check

## 📚 Documentation

- **[Quick Start Guide](QUICKSTART.md)** - Step-by-step setup instructions
- **[Architecture Documentation](ARCHITECTURE.md)** - Design decisions and patterns
- **[Contributing Guidelines](CONTRIBUTING.md)** - How to contribute

## 🤝 Contributing

We welcome contributions! Please see [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

Built with:
- .NET 8
- Entity Framework Core
- Serilog
- Elastic Stack (Elasticsearch, Kibana, APM)
- Code Analyzers (SonarAnalyzer, Meziantou, Roslynator)

---

**Made with ❤️ for the .NET community**

Ready to start your next project? Clone this template and start building! 🚀
