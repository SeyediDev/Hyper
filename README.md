# Hyper Backend

**Customer Loyalty Platform - Backend Services**

🔒 **Private Repository**

## Overview

Hyper Backend provides the core business logic and APIs for the Customer Loyalty Platform, built on **Neo Framework**.

## Architecture

```
Hyper.Backend/
├─ Hyper.Domain              → Domain entities, value objects
├─ Hyper.Application         → CQRS commands/queries, business logic
├─ Hyper.Infrastructure      → Data access, external services
├─ Hyper.Channel.Application → Channel-specific application logic
├─ Hyper.Channel.Api         → REST API endpoints
└─ Hyper.Bpms               → BPMN definitions and workflows
```

## Features

### Core Modules
- 👥 **Customer Management** - Customer profiles, segmentation
- 🎁 **Points & Rewards** - Point calculation, redemption
- 📦 **Products & Assets** - Product catalog, digital assets
- 🎲 **Promotions & Lottery** - Campaign management, lottery system
- 📊 **Scoring Rules** - Dynamic point calculation
- 📝 **Surveys & Feedback** - Customer surveys, feedback management
- 💬 **Forum & Community** - Discussion forums, Q&A

### Technical Features
- ✅ Clean Architecture with DDD
- ✅ CQRS with MediatR
- ✅ Built on Neo Framework
- ✅ Entity Framework Core
- ✅ Keycloak Authentication
- ✅ SMS Integration
- ✅ Background Jobs (Hangfire)
- ✅ Redis Caching

## Technology Stack

- **.NET 8.0**
- **Neo Framework** (Infrastructure)
- **SQL Server / PostgreSQL**
- **Redis**
- **Keycloak**
- **RabbitMQ**

## Getting Started

### Prerequisites

```bash
- .NET 8 SDK
- Docker & Docker Compose
- SQL Server or PostgreSQL
- Redis
- Keycloak
```

### Configuration

```bash
# 1. Clone repository
git clone https://github.com/Hyper-Apps/Hyper-Backend.git
cd Hyper-Backend

# 2. Restore packages
dotnet restore

# 3. Update connection strings in appsettings.json
# 4. Run migrations
dotnet ef database update --project src/Hyper.Infrastructure

# 5. Run API
cd src/Hyper.Channel.Api
dotnet run
```

### Environment Variables

```bash
ConnectionStrings__DefaultConnection=...
Keycloak__Authority=...
Redis__Configuration=...
```

## API Documentation

API documentation available at:
- Swagger: `https://localhost:5001/swagger`
- OpenAPI: `https://localhost:5001/swagger/v1/swagger.json`

## Development

### Build

```bash
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Database Migrations

```bash
# Add migration
dotnet ef migrations add MigrationName --project src/Hyper.Infrastructure

# Update database
dotnet ef database update --project src/Hyper.Infrastructure
```

## Project Structure

```
src/
├─ Hyper.Domain/
│  ├─ Entities/          → Domain entities
│  ├─ ValueObjects/      → Value objects
│  ├─ Events/            → Domain events
│  └─ Repository/        → Repository interfaces
│
├─ Hyper.Application/
│  ├─ Features/          → CQRS features
│  │  ├─ Account/
│  │  ├─ Hyper/
│  │  ├─ Feedback/
│  │  ├─ Forum/
│  │  └─ Surveys/
│  └─ DependencyInjection.cs
│
├─ Hyper.Infrastructure/
│  ├─ Data/              → EF Core, repositories
│  └─ Features/          → External services
│
└─ Hyper.Channel.Api/
   ├─ Controllers/       → API endpoints
   └─ Program.cs
```

## Contributing

This is a private repository. For contribution guidelines, see [CONTRIBUTING.md](CONTRIBUTING.md).

## License

**Proprietary** - All rights reserved.

---

© 2024 Hyper Platform


