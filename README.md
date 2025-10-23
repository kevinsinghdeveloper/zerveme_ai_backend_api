# ZerveMeData API

A multi-tenant data management and analytics platform built with ASP.NET Core 8.0.

## Overview

ZerveMeData is a comprehensive data analytics platform that provides API services for managing datasets, data warehouses, reports, and analytics jobs. It features multi-tenant architecture with organization-based data isolation, JWT authentication, and support for multiple data warehouse backends.

## Architecture

### Solution Structure

- **WebApi** - ASP.NET Core Web API (Entry point)
- **Core** - Business logic and service layer
- **Core.Contracts** - Interface definitions and abstractions
- **Data** - Data access layer with Entity Framework Core
- **Utilities** - Shared utilities
- **ZerveMeData.Tests.Core** - Unit/integration tests

## Technology Stack

- .NET 8.0
- ASP.NET Core Web API
- Entity Framework Core 7.0.4
- SQL Server (primary database)
- Google BigQuery & PostgreSQL (data warehouse support)
- ASP.NET Identity with JWT Bearer authentication
- Swagger/OpenAPI for API documentation

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- SQL Server
- AWS Account (for S3 storage)
- Google Cloud Project (for BigQuery, optional)
- PostgreSQL (for PostgreSQL data warehouse, optional)

### Configuration

1. **Copy the configuration template:**
   ```bash
   cd WebApi
   cp appsettings.template.json appsettings.json
   cp appsettings.template.json appsettings.Development.json
   ```

2. **Configure your settings:**

   Edit `appsettings.json` with your actual credentials:
   - AWS Access Key and Secret Key
   - JWT Secret Key (generate with `openssl rand -base64 32`)
   - Database connection strings
   - PostgreSQL connection (if using)

3. **Using .NET User Secrets (Recommended for Development):**
   ```bash
   cd WebApi
   dotnet user-secrets init
   dotnet user-secrets set "AWS:AWSAccessKey" "your-key"
   dotnet user-secrets set "AWS:AWSSecretKey" "your-secret"
   dotnet user-secrets set "JWT:SecretKey" "your-jwt-secret"
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
   ```

### Database Setup

1. **Update the connection string** in `appsettings.json` or user secrets

2. **Run migrations:**
   ```bash
   cd WebApi
   dotnet ef database update
   ```

### Running the Application

```bash
cd WebApi
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

## Core Features

### Multi-Tenant Architecture
- Organization-based data isolation
- User-organization relationship tracking
- Row-level security

### Data Warehouse Integration
- Google BigQuery support
- PostgreSQL support
- Abstracted warehouse handler for easy extension

### Dataset Management
- Domain model with KPI and attribute columns
- Custom SQL or table-based queries
- Aggregation support (Sum, Average, Count, Min, Max)
- Query performance tracking

### Job Scheduling
- Background worker service for scheduled tasks
- Support for recurring and one-off jobs
- Configurable job frequencies
- Job status tracking

### Authentication & Authorization
- ASP.NET Identity
- JWT Bearer tokens
- Role-based authorization (User, Admin, SuperAdmin)
- Policy-based access control

## API Endpoints

The API provides the following main controllers:

- `/api/authentication` - User authentication and registration
- `/api/datasets` - Dataset CRUD operations
- `/api/dwh` - Data warehouse management
- `/api/jobs` - Job scheduling and management
- `/api/models` - ML model management
- `/api/organizations` - Organization management
- `/api/projects` - Project management
- `/api/reports` - Report operations
- `/api/users` - User management

Full API documentation available at `/swagger` when running the application.

## Security

### Important Security Notes

- **Never commit `appsettings.json` files** with real credentials to git
- The `.gitignore` is configured to exclude all `appsettings.json` files
- Use `.NET User Secrets` for development
- Use Azure Key Vault, AWS Secrets Manager, or environment variables in production
- The current configuration has weak password requirements for development - strengthen for production

### Production Deployment

For production:
1. Use a secrets management service (Azure Key Vault, AWS Secrets Manager)
2. Configure CORS to restrict allowed origins
3. Enable HTTPS redirection
4. Strengthen password policies
5. Use connection string encryption
6. Enable detailed logging and monitoring

## Development

### Building the Solution

```bash
dotnet build
```

### Running Tests

```bash
dotnet test
```

### Database Migrations

Create a new migration:
```bash
cd WebApi
dotnet ef migrations add YourMigrationName
```

Apply migrations:
```bash
dotnet ef database update
```

## Project Structure

```
zervemedata/
├── Core/                           # Business logic
│   ├── Services/                   # Service implementations
│   │   ├── BackgroundWorkers/      # Job scheduler
│   │   └── DataWarehouseManagers/  # DWH integrations
│   └── Extensions/                 # Extension methods
├── Core.Contracts/                 # Interfaces and contracts
│   └── Abstractions/               # Interface definitions
├── Data/                           # Data access layer
│   ├── Entities/                   # Database entities
│   ├── Configurations/             # EF Core configurations
│   ├── DataModels/                 # DTOs and data models
│   └── Enumerations/               # Enums
├── WebApi/                         # Web API project
│   ├── Controllers/                # API controllers
│   ├── Migrations/                 # EF Core migrations
│   └── appsettings.template.json   # Configuration template
├── Utilities/                      # Shared utilities
└── ZerveMeData.Tests.Core/         # Tests
```

## Contributing

1. Create a feature branch from `develop`
2. Make your changes
3. Ensure tests pass
4. Submit a pull request

## License

[Your License Here]

## Support

For issues and questions, please create an issue in the repository.