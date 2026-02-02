# Architecture Overview

This document describes the architecture of the .NET 8 API template. For visual diagrams, use these descriptions with your preferred diagramming tool.

## System Architecture with Observability

### Request Flow

The application follows a layered architecture where requests flow from top to bottom:

1. **User/Client** - Makes HTTP requests
2. **API Gateway/Load Balancer** - Routes traffic
3. **API Layer (Controllers)** 
   - ProductsController handles REST endpoints
   - GET /api/products
   - POST /api/products
   - PUT /api/products/{id}
   - DELETE /api/products/{id}
4. **Application Layer (Services)**
   - ProductService contains business logic
   - Handles validation and orchestration
5. **Infrastructure Layer (Data Access)**
   - Repository<T> pattern
   - UnitOfWork for transactions
   - ApplicationDbContext (EF Core)
6. **SQL Server Database** - Data persistence

### Observability Stack

The application includes comprehensive observability:

**Within the .NET API Application:**
- **Serilog** - Structured logging with ECS format and enrichers
- **Elastic APM Agent** - Auto-instrumentation, tracing, and error capture

**Data Flow:**
- Serilog sends logs (ECS JSON format) to Elasticsearch
- APM Agent sends transactions, spans, and metrics to APM Server
- APM Server processes and forwards data to Elasticsearch
- Kibana queries and visualizes all data from Elasticsearch

**Kibana Features:**
- Discover: View logs in real-time
- APM: Analyze application performance
- Dashboard: Create custom visualizations
- Alerts: Configure notifications

## Code Quality Pipeline

### Developer Workflow

**1. Write Code**
- IDE (VS Code / Visual Studio)
- Real-time code analysis
- IntelliSense warnings
- Quick fixes available

**2. Build**
- Roslyn Compiler runs with three code analyzers:
  - **SonarAnalyzer.CSharp** - Detects security issues, bugs, and code smells
  - **Meziantou.Analyzer** - Enforces best practices and performance patterns
  - **Roslynator.Analyzers** - Provides code quality rules and style enforcement
- Build results: Errors (fails), Warnings (succeeds with warnings), or Success

**3. Test**
- `dotnet test` runs all tests
- Unit tests, integration tests
- Coverage reports generated

**4. Deploy**
- CI/CD Pipeline executes:
  - Build
  - Analyze
  - Test
  - Package
  - Deploy

## Data Flow

### Request Flow with Observability

When an HTTP request arrives, it flows through the middleware pipeline:

1. **Request Received**
   - Elastic APM starts a transaction
   - Serilog logs request details (method, path, headers)

2. **Controller Action**
   - APM creates a span labeled "Controller"

3. **Service Layer**
   - APM creates a span labeled "Service"
   - Serilog logs business operations

4. **Repository Layer**
   - APM creates a span labeled "Database"

5. **Entity Framework**
   - APM captures SQL queries and their performance

6. **SQL Server**
   - Database executes the query

7. **Response Generated**
   - APM ends the transaction with status code
   - Serilog logs response (status, duration)

8. **HTTP Response** sent back to client

### Log Flow

**Application Code** calls `Serilog.Log.Information(...)`

↓

**Serilog Pipeline** processes the log:
1. Enriches with context (timestamp, level, logger, machine name, thread ID, correlation ID)
2. Formats as ECS JSON
3. Writes to multiple sinks simultaneously:
   - **Console Sink** - Terminal output for development
   - **File Sink** - logs/api-{date}.txt for local persistence
   - **Elasticsearch Sink** - Cloud storage for centralized logging

↓

**Elasticsearch** receives and indexes logs (e.g., apitemplate-logs-2026.02.02)

↓

**Kibana** provides search, visualization, and dashboard capabilities

## Clean Architecture Layers

The application follows Clean Architecture with clear dependency rules:

### Layer Structure (top to bottom):

**1. API Layer (Presentation)**
- Contains Controllers
- Example: ProductsController
- Responsibilities: Handle HTTP requests/responses, validate input, return DTOs
- Dependencies: References Application layer

**2. Application Layer (Business Logic)**
- Contains Services, DTOs, and Interfaces
- Example: ProductService (implements IProductService)
- Responsibilities: Business rules, validation, orchestration
- Contains: CreateProductDto, ProductDto, etc.
- Defines: IRepository<T>, IUnitOfWork interfaces
- Dependencies: References Domain layer only

**3. Domain Layer (Core Business)**
- Contains Entities and Common types
- Example: Product entity, BaseEntity
- Responsibilities: Pure business objects
- Dependencies: NONE (completely independent)

**4. Infrastructure Layer (Data & External Services)**
- Contains Data access and Repository implementations
- Includes: ApplicationDbContext, entity configurations
- Implements: Repository<T>, UnitOfWork, EF Core integration
- Dependencies: References Domain and Application (implements interfaces from Application)

### Dependency Flow

- **API Layer** depends on **Application Layer**
- **Application Layer** depends on **Domain Layer**
- **Infrastructure Layer** implements interfaces from **Application Layer**
- **Infrastructure Layer** references **Domain Layer** entities
- **Domain Layer** has NO dependencies (core principle)

## Technology Stack Summary

| Layer | Technologies |
|-------|-------------|
| **API** | ASP.NET Core 8, Swagger/OpenAPI, Serilog |
| **Application** | C# Services, DTOs, Interfaces |
| **Domain** | C# Entities, Business Logic |
| **Infrastructure** | EF Core, SQL Server, Repository Pattern |
| **Observability** | Elasticsearch, Kibana, APM Server, Serilog |
| **Code Quality** | SonarAnalyzer, Meziantou, Roslynator |
| **Container** | Docker, Docker Compose |
| **Testing** | xUnit, Moq, FluentAssertions |

## Key Design Patterns

- **Clean Architecture**: Separation of concerns with clear boundaries
- **Repository Pattern**: Abstract data access
- **Unit of Work**: Manage transactions and consistency
- **Dependency Injection**: Loose coupling and testability
- **DTOs**: Separate internal models from external contracts
- **Middleware Pipeline**: Request/response processing
- **Structured Logging**: Searchable, contextual logs
- **APM**: Distributed tracing and performance monitoring

## Benefits of This Architecture

✅ **Maintainability**: Clear separation of concerns  
✅ **Testability**: Easy to unit test with DI  
✅ **Scalability**: Layers can scale independently  
✅ **Observability**: Full visibility into application behavior  
✅ **Code Quality**: Automated analysis and enforcement  
✅ **Flexibility**: Easy to swap implementations  
✅ **Production-Ready**: Monitoring and logging built-in

---

## Diagram Generation Prompts

To generate visual diagrams, use these prompts with your preferred tool (Mermaid, PlantUML, draw.io, etc.):

### System Architecture Diagram
"Create a vertical flow diagram showing: User/Client → API Gateway → API Layer (with ProductsController) → Application Layer (with ProductService) → Infrastructure Layer (with Repository and UnitOfWork) → SQL Server Database"

### Observability Stack Diagram
"Create a diagram showing: .NET API Application (containing Serilog and APM Agent) sending data to Elasticsearch and APM Server, which both feed into Kibana for visualization"

### Clean Architecture Layers Diagram
"Create a layered architecture diagram showing 4 layers stacked vertically: API Layer (top), Application Layer, Domain Layer, and Infrastructure Layer (bottom). Show dependency arrows: API→Application, Application→Domain, Infrastructure→Domain and Infrastructure implements Application interfaces"

### Request Flow Diagram
"Create a sequence diagram showing HTTP request flow through: Request → APM Start → Controller → Service → Repository → EF Core → SQL Server → Response, with APM and Serilog logging at each step"
