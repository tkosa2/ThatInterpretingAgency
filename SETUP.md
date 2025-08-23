# THAT Interpreting Agency - Setup Guide

This guide will help you set up and run the THAT Interpreting Agency application.

## Prerequisites

- **.NET 9.0 SDK** - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/9.0)
- **SQL Server** - Local instance or Azure SQL
- **Visual Studio Code** (recommended) with C# extension

## Database Setup

1. **Create Database**
   ```sql
   CREATE DATABASE ThatInterpretingAgency;
   ```

2. **Run Schema Script**
   - Open the file: `src/Infrastructure/Persistence/Migrations/update_schema.sql`
   - Execute it against your SQL Server instance
   - This will create all tables and insert the default agency

## Configuration

1. **Update Connection String**
   - Open `src/Api/appsettings.json`
   - Update the connection string if needed:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost;Database=ThatInterpretingAgency;Trusted_Connection=true;MultipleActiveResultSets=true"
   }
   ```

2. **QuickBooks Configuration** (Optional for development)
   - Update the QuickBooks section in `appsettings.json` with your credentials
   - For development, the mock service will work without real credentials

## Building and Running

1. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

2. **Build the Solution**
   ```bash
   dotnet build
   ```

3. **Run the API**
   ```bash
   cd src/Api
   dotnet run
   ```

4. **Run Tests**
   ```bash
   dotnet test
   ```

## API Endpoints

The application will be available at `https://localhost:5001` (or the port shown in the console).

### Available Endpoints:

- **POST** `/api/agencies` - Create a new agency
- **GET** `/api/agencies` - List all agencies
- **GET** `/api/agencies/{id}` - Get agency by ID

- **POST** `/api/appointments` - Book an appointment
- **GET** `/api/appointments` - List all appointments
- **GET** `/api/appointments/available-interpreters` - Find available interpreters

- **POST** `/api/billing/invoices` - Create an invoice
- **GET** `/api/billing/invoices` - List all invoices

## Testing the Application

1. **Create an Agency**
   ```bash
   curl -X POST "https://localhost:5001/api/agencies" \
        -H "Content-Type: application/json" \
        -d '{
          "name": "My Interpreting Agency",
          "contactInfo": "contact@myagency.com"
        }'
   ```

2. **Book an Appointment**
   ```bash
   curl -X POST "https://localhost:5001/api/appointments" \
        -H "Content-Type: application/json" \
        -d '{
          "agencyId": "your-agency-id",
          "interpreterId": "your-interpreter-id",
          "clientId": "your-client-id",
          "startTime": "2024-01-15T10:00:00Z",
          "endTime": "2024-01-15T12:00:00Z",
          "timeZone": "UTC",
          "location": "Conference Room A",
          "language": "Spanish"
        }'
   ```

## Project Structure

```
ThatInterpretingAgency/
├── src/
│   ├── Core/                   # Domain and Application logic
│   │   ├── Domain/            # DDD entities, aggregates
│   │   └── Application/       # CQRS commands/queries
│   ├── Infrastructure/         # EF Core, QuickBooks integration
│   │   ├── Persistence/       # MSSQL DbContext, migrations
│   │   └── Services/          # QuickBooksService, etc.
│   └── Api/                   # ASP.NET Core API
├── tests/
│   ├── UnitTests/            # xUnit tests for domain logic
│   ├── IntegrationTests/     # Tests for MSSQL, QuickBooks
│   └── EndToEndTests/        # API and flow tests
└── README.md
```

## Development Notes

- **Domain-Driven Design**: The application follows DDD principles with aggregates, entities, and value objects
- **CQRS Pattern**: Uses MediatR for command/query separation
- **Vertical Slices**: Each feature is implemented as a vertical slice
- **Multi-tenancy**: Agencies are the top-level entity for data isolation
- **QuickBooks Integration**: Mock service for development, real API for production

## Troubleshooting

1. **Database Connection Issues**
   - Verify SQL Server is running
   - Check connection string in `appsettings.json`
   - Ensure database exists

2. **Build Errors**
   - Run `dotnet restore` to restore packages
   - Ensure .NET 9.0 SDK is installed
   - Check for any missing dependencies

3. **Runtime Errors**
   - Check application logs in the console
   - Verify database schema was created correctly
   - Ensure all required services are configured

## Next Steps

- Implement authentication and authorization
- Add more comprehensive validation
- Implement real QuickBooks API integration
- Add background services for notifications
- Implement comprehensive testing
- Add logging and monitoring
- Set up CI/CD pipeline

For more information, see the main README.md file.
