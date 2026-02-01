# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["ApiTemplate.sln", "."]
COPY ["src/ApiTemplate.Domain/ApiTemplate.Domain.csproj", "src/ApiTemplate.Domain/"]
COPY ["src/ApiTemplate.Application/ApiTemplate.Application.csproj", "src/ApiTemplate.Application/"]
COPY ["src/ApiTemplate.Infrastructure/ApiTemplate.Infrastructure.csproj", "src/ApiTemplate.Infrastructure/"]
COPY ["src/ApiTemplate.API/ApiTemplate.API.csproj", "src/ApiTemplate.API/"]
COPY ["tests/ApiTemplate.Tests/ApiTemplate.Tests.csproj", "tests/ApiTemplate.Tests/"]

# Restore dependencies
RUN dotnet restore

# Copy everything else
COPY . .

# Build
WORKDIR "/src/src/ApiTemplate.API"
RUN dotnet build "ApiTemplate.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "ApiTemplate.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApiTemplate.API.dll"]
