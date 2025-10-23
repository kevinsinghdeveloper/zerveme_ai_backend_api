# Configuration Setup

## Setting up appsettings.json

This project requires an `appsettings.json` file that is not tracked in git for security reasons.

1. Copy `appsettings.template.json` to `appsettings.json`
2. Copy `appsettings.template.json` to `appsettings.Development.json`
3. Fill in the actual values for:
   - AWS credentials
   - JWT secret key
   - Database connection strings

**NEVER commit appsettings.json files with real credentials to git!**

## Using User Secrets (Recommended for Development)

For better security in development, use .NET User Secrets:

```bash
cd WebApi
dotnet user-secrets init
dotnet user-secrets set "AWS:AWSAccessKey" "your-key"
dotnet user-secrets set "AWS:AWSSecretKey" "your-secret"
dotnet user-secrets set "JWT:SecretKey" "your-jwt-secret"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
```

## Production Configuration

For production, use:
- Azure Key Vault
- AWS Secrets Manager
- Environment variables
- Any other secure secret management system