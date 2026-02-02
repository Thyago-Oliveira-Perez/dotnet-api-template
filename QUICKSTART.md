# Quick Start Guide

Get your API running in under 5 minutes! Choose your preferred setup method below.

## 📋 Prerequisites

**Choose one:**

**Option A: Docker (Recommended)**
- Docker Desktop installed
- At least 4GB RAM allocated to Docker

**Option B: Local Development**
- [mise](https://mise.jdx.dev/) OR .NET 8 SDK installed manually
- SQL Server (local or remote)

## 🚀 Option A: Docker Setup (Full Stack)

This is the easiest way to get everything running, including the observability stack.

### Step 1: Clone and Start

```bash
# Clone the repository
git clone https://github.com/Thyago-Oliveira-Perez/dotnet-api-template
cd dotnet-api-template

# Start all services
docker-compose up -d
```

### Step 2: Wait for Services

```bash
# Services take ~2 minutes to initialize
# Check status
docker-compose ps

# Watch logs
docker-compose logs -f
```

### Step 3: Verify

```bash
# Check health
curl http://localhost:5000/health

# Try the API
curl http://localhost:5000/api/products
```

### Step 4: Explore

- **Swagger UI**: http://localhost:5000/swagger
- **Kibana (Logs)**: http://localhost:5601
- **Health Check**: http://localhost:5000/health

### Services Included

| Service | URL | Purpose |
|---------|-----|---------|
| API | http://localhost:5000 | REST API |
| Swagger | http://localhost:5000/swagger | API Docs |
| SQL Server | localhost:1433 | Database |
| Kibana | http://localhost:5601 | Logs & APM |
| Elasticsearch | http://localhost:9200 | Log Storage |

### Common Docker Issues

**Port Already in Use**
```bash
# Find process using port
sudo lsof -i :5000
sudo lsof -i :9200

# Change port in docker-compose.yml
ports:
  - "5001:8080"  # Instead of 5000
```

**Out of Memory**
```bash
# Increase Docker memory
# Docker Desktop → Settings → Resources → Memory → 4GB+

# Or reduce Elasticsearch memory in docker-compose.yml
environment:
  - "ES_JAVA_OPTS=-Xms512m -Xmx512m"
```

**Services Not Starting**
```bash
# Check logs
docker-compose logs elasticsearch
docker-compose logs kibana

# Restart specific service
docker-compose restart elasticsearch

# Full restart
docker-compose down
docker-compose up -d
```

**Kibana Can't Connect**
```bash
# Wait 2-3 minutes for Elasticsearch to be ready
# Check Elasticsearch health
curl http://localhost:9200/_cluster/health

# Should return: "status":"green" or "status":"yellow"
```

**Elasticsearch Won't Start**
```bash
# Check if port 9200 is already in use
sudo lsof -i :9200

# Increase vm.max_map_count on Linux
sudo sysctl -w vm.max_map_count=262144

# Make it permanent
echo "vm.max_map_count=262144" | sudo tee -a /etc/sysctl.conf
```

**API Can't Connect to Database**
```bash
# Check SQL Server is running
docker-compose ps sqlserver

# Check connection string in appsettings.json
# It should match docker-compose.yml settings
```

## 🔧 Option B: Local Development

Run the API locally without Docker. You'll get console/file logging but no Kibana/APM.

### Step 1: Setup .NET

**Using mise (Recommended):**
```bash
cd dotnet-api-template
mise trust
mise install  # Installs .NET 8 from .mise.toml
```

**Or install manually:**
- Download .NET 8 SDK from https://dotnet.microsoft.com/download

### Step 2: Configure Database

Edit `src/ApiTemplate.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ApiTemplate;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  }
}
```

### Step 3: Run Migrations

```bash
# Install EF Core tools (one-time)
dotnet tool install --global dotnet-ef

# Create database and tables
dotnet ef database update \
  --project src/ApiTemplate.Infrastructure \
  --startup-project src/ApiTemplate.API
```

### Step 4: Run the API

```bash
# Build
dotnet build

# Run
dotnet run --project src/ApiTemplate.API
```

### Step 5: Test

```bash
# In a new terminal
curl http://localhost:5000/health
curl http://localhost:5000/api/products
```

Open http://localhost:5000/swagger in your browser.

## 📊 Viewing Logs

### With Docker (Kibana)

1. Open http://localhost:5601
2. Wait for Kibana to initialize (~2 minutes)
3. Go to **Discover** tab
4. Create index pattern: `apitemplate-logs-*`
5. View real-time logs with filtering and search

### Local Development (Console/File)

**Console Output:**
```bash
# Logs appear in your terminal
dotnet run --project src/ApiTemplate.API
```

**File Output:**
- Location: `logs/api-{date}.txt`
- Format: JSON (ECS format)

## 🧪 Running Tests

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"

# Run specific test
dotnet test --filter "FullyQualifiedName~ProductService"
```

## 🔄 Making Your First Change

### 1. Add a New Entity

Create `src/ApiTemplate.Domain/Entities/Category.cs`:

```csharp
using ApiTemplate.Domain.Common;

namespace ApiTemplate.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
```

### 2. Update DbContext

Edit `src/ApiTemplate.Infrastructure/Data/ApplicationDbContext.cs`:

```csharp
public DbSet<Category> Categories { get; set; }
```

### 3. Create Migration

```bash
dotnet ef migrations add AddCategoryTable \
  --project src/ApiTemplate.Infrastructure \
  --startup-project src/ApiTemplate.API

dotnet ef database update \
  --project src/ApiTemplate.Infrastructure \
  --startup-project src/ApiTemplate.API
```

### 4. Add Service

Create `src/ApiTemplate.Application/Services/CategoryService.cs`:

```csharp
// Similar to ProductService.cs
```

### 5. Add Controller

Create `src/ApiTemplate.API/Controllers/CategoriesController.cs`:

```csharp
// Similar to ProductsController.cs
```

### 6. Test

```bash
# Run tests
dotnet test

# Try the new endpoint
curl http://localhost:5000/api/categories
```

## 📖 Next Steps

### Learn the Architecture
- Read [ARCHITECTURE.md](ARCHITECTURE.md) to understand the project structure
- Explore the sample Product implementation
- Review the Repository and Unit of Work patterns

### Explore Observability
- Make some API requests: `curl http://localhost:5000/api/products`
- Open Kibana: http://localhost:5601
- View logs in **Discover**
- Check performance in **Observability → APM**

### Improve Code Quality
- Run `dotnet build` and review analyzer warnings
- Check `.editorconfig` for code style rules
- Fix warnings to improve code quality

### Deploy to Production
- Review production configurations in `appsettings.json`
- Set up environment variables for secrets
- Configure connection strings for production database
- Set up proper Elasticsearch security

## 💡 Tips

1. **Use Docker for first run** - Easiest way to see everything working
2. **Check health endpoint** - `curl http://localhost:5000/health`
3. **View Swagger docs** - http://localhost:5000/swagger
4. **Monitor logs in Kibana** - Better than console logs
5. **Run tests frequently** - `dotnet test`
6. **Use mise for version management** - Keeps .NET version consistent

## 🆘 Getting Help

**Something not working?**

1. Check health endpoint: `curl http://localhost:5000/health`
2. View Docker logs: `docker-compose logs -f`
3. Check this guide's troubleshooting section
4. Open an issue on GitHub
5. Review [CONTRIBUTING.md](CONTRIBUTING.md)

## 📚 Additional Resources

- [.NET 8 Documentation](https://docs.microsoft.com/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [Elasticsearch Documentation](https://www.elastic.co/guide/)
- [Docker Documentation](https://docs.docker.com/)

---

**Ready to build? Start coding!** 🚀

For more details, see the main [README.md](README.md) or [ARCHITECTURE.md](ARCHITECTURE.md).
