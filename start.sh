#!/bin/bash

# Quick Start Script for .NET API Template

echo "🚀 .NET 8 API Template - Quick Start"
echo "===================================="
echo ""

# Check if mise is installed
if ! command -v mise &> /dev/null; then
    echo "⚠️  mise not found. Please install mise or ensure .NET 8 SDK is installed."
    echo "   Install mise: https://mise.jdx.dev/"
    exit 1
fi

# Trust and install .NET via mise
echo "📦 Setting up .NET 8 via mise..."
mise trust
mise install

# Build the solution
echo ""
echo "🔨 Building the solution..."
dotnet build

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Build successful!"
    echo ""
    echo "📝 Next steps:"
    echo "   1. Update the connection string in src/ApiTemplate.API/appsettings.json"
    echo "   2. Run migrations: dotnet ef database update --project src/ApiTemplate.Infrastructure --startup-project src/ApiTemplate.API"
    echo "   3. Start the API: dotnet run --project src/ApiTemplate.API/ApiTemplate.API.csproj"
    echo ""
    echo "   Or use Docker Compose: docker-compose up --build"
    echo ""
    echo "   Then navigate to: http://localhost:5000/swagger"
else
    echo ""
    echo "❌ Build failed. Please check the errors above."
    exit 1
fi
