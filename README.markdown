# THAT Interpreting Agency

## Overview

THAT Interpreting Agency is a global platform for managing interpreting services, including interpreter profiles, client bookings, appointment scheduling, billing, and notifications. Built with **ASP.NET Core 9**, it uses **Domain-Driven Design (DDD)** with **vertical slice architecture** to ensure modularity and scalability. The application supports multi-tenancy, with **Agencies** as the top-level entity, and integrates with **QuickBooks Online** for invoicing and payment processing. The database is **Microsoft SQL Server (MSSQL)**, and the project is developed using **Visual Studio Code (VS Code)** with free extensions.

### Key Features
- **Multi-Tenancy**: Agencies (e.g., THAT Interpreting Agency) manage interpreters, clients, and appointments.
- **Scheduling**: Book appointments with time zone support and availability checks.
- **Billing**: Create invoices via QuickBooks API, track payments, and pay interpreters.
- **Notifications**: Send email/SMS reminders for appointments.
- **Security**: Role-based access control and agency-scoped data access.
- **Scalability**: Designed for global operations, handling thousands of appointments daily.

### Architecture
- **Framework**: ASP.NET Core 9
- **Architecture**: DDD with vertical slices, CQRS (via MediatR), EF Core for MSSQL
- **Bounded Contexts**: Agency Management, Interpreter Management, Client Management, Scheduling, Billing, Notifications
- **Database**: MSSQL with multi-tenant support via AgencyId
- **External Integrations**: QuickBooks Online API for invoicing and payments

## Requirements

### Database Schema
The application uses an MSSQL database with the following entities and relationships:

- **Agencies**: Top-level entity (Id: UNIQUEIDENTIFIER, Name: NVARCHAR(100), ContactInfo: NVARCHAR(500), CreatedAt: DATETIME2, UpdatedAt: DATETIME2)
- **AgencyStaff**: Junction table for Many-to-Many User-Agency relationship (AgencyId, UserId, Role: NVARCHAR(50), HireDate: DATETIME2, Status: NVARCHAR(20))
- **Users**: Managed by ASP.NET Core Identity (AspNetUsers: Id, Username, Email, PasswordHash)
- **Interpreters**: Linked to Users and Agencies (Id, UserId, AgencyId, FullName)
- **Clients**: Linked to Users and Agencies (Id, UserId, AgencyId, OrganizationName, Preferences)
- **Appointments**: Tracks bookings (Id, AgencyId, InterpreterId, ClientId, TimeSlotStart: DATETIME2, TimeSlotEnd: DATETIME2, Status)
- **Invoices**: References QuickBooks invoices (Id, AgencyId, ClientId, AppointmentId, QuickBooksInvoiceId: NVARCHAR(50), CreatedAt)
- **Notifications**: Tracks messages (Id, AgencyId, UserId, Type, Message, SentAt)

**Constraints**:
- Foreign keys with `ON DELETE CASCADE` for data integrity.
- Indexes on `AgencyId` for performance.
- No EIN/SSN fields; tax data is managed by QuickBooks.

**SQL Schema Script**:
See `src/Infrastructure/Persistence/Migrations/update_schema.sql` for the MSSQL schema creation and data migration (e.g., assigning existing data to a default agency named 'THAT Interpreting Agency').

### User Stories
- **As an Agency Admin**, I want to create and manage interpreter profiles for THAT Interpreting Agency so that I can assign certified interpreters to global clients.
  - **Acceptance Criteria**: Create interpreter with name, skills, and agency association; validate AgencyId.
- **As an Interpreter**, I want to update my availability across time zones so that I can be scheduled without conflicts.
  - **Acceptance Criteria**: Save availability slots in MSSQL; prevent overlapping appointments.
- **As a Client**, I want to book an appointment with filters for language skills and location so that I can find suitable interpreters.
  - **Acceptance Criteria**: API returns available interpreters in the same agency; booking creates an appointment record.
- **As an Agency Admin**, I want to trigger invoice creation in QuickBooks for completed appointments so that clients receive bills automatically.
  - **Acceptance Criteria**: Send appointment details to QuickBooks API, store QuickBooksInvoiceId in MSSQL, validate AgencyId.
- **As a Client**, I want to receive invoices from QuickBooks so that I can pay securely.
  - **Acceptance Criteria**: QuickBooks sends invoice email with payment link; local system logs invoice reference.
- **As an Interpreter**, I want QuickBooks to handle my payments so that I receive funds for completed appointments.
  - **Acceptance Criteria**: QuickBooks initiates payment after client payment; no local payment storage.

### Tests
- **Unit Tests** (xUnit):
  - Test `InvoiceAggregate.CreateQuickBooksInvoiceReference`: Ensure QuickBooksInvoiceId and AgencyId are stored correctly.
  - Test `AppointmentAggregate.Schedule`: Validate no overlaps and correct agency scoping.
- **Integration Tests**:
  - Test `CreateInvoiceCommandHandler`: Mock QuickBooks API, verify MSSQL storage of invoice reference.
  - Test `ScheduleRepository`: Ensure MSSQL queries filter by AgencyId.
- **End-to-End Tests**:
  - Test POST /api/billing/invoices: Verify QuickBooks API call and MSSQL update.
  - Test POST /api/scheduling/appointments: Ensure appointment creation and notification.

### Other Requirements
- **Functional**:
  - QuickBooks Online API integration for invoices and payments.
  - Multi-language support for global users.
  - Email/SMS notifications via external services (e.g., SendGrid, Twilio).
- **Non-Functional**:
  - **Performance**: Handle 10,000 appointments/day, QuickBooks API calls <2s.
  - **Security**: OAuth 2.0 for QuickBooks, JWT for API auth, GDPR compliance.
  - **Scalability**: MSSQL partitioning by AgencyId for global ops.
- **Architecture**:
  - Vertical slices for modularity (e.g., Billing slice with CreateInvoiceCommand).
  - CQRS with MediatR, EF Core for MSSQL.
  - Keep codebase lean (<500,000 lines) for AI tool performance.

## Development Setup

### Prerequisites
- **Visual Studio Code**: Free IDE for development.
- **.NET SDK 9.0**: For ASP.NET Core 9.
- **MSSQL Server**: Local or cloud instance (e.g., Azure SQL).
- **QuickBooks Online Developer Account**: For API access.
- **Git**: For version control.

### Recommended VS Code Extensions
- **C# (ms-dotnettools.csharp)**: IntelliSense, debugging for C#.
- **SQL Server (ms-mssql.mssql)**: Manage MSSQL databases and run schema scripts.
- **C# XML Documentation Comments (k--kato.docomment)**: Add code documentation.
- **Markdown All in One (yzhang.markdown-all-in-one)**: Edit README.md.
- **REST Client (humao.rest-client)**: Test QuickBooks and app APIs.
- **GitLens (eamodio.gitlens)**: Enhanced Git integration.
- **Prettier (esbenp.prettier-vscode)**: Format C# and Markdown.
- **Docker (ms-azuretools.vscode-docker)**: For containerized deployment.

### Setup Instructions
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/your-repo/that-interpreting-agency.git
   cd that-interpreting-agency
   ```
2. **Install .NET SDK**:
   - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/9.0).
3. **Set Up MSSQL**:
   - Create a database: `CREATE DATABASE ThatInterpretingAgency;`
   - Apply the schema: Run `src/Infrastructure/Persistence/Migrations/update_schema.sql` using the SQL Server extension.
4. **Configure QuickBooks**:
   - Create a QuickBooks Online app at [developer.intuit.com](https://developer.intuit.com).
   - Store OAuth credentials in `appsettings.json`:
     ```json
     {
       "QuickBooks": {
         "ClientId": "your-client-id",
         "ClientSecret": "your-client-secret",
         "RealmId": "your-realm-id"
       }
     }
     ```
5. **Restore and Run**:
   ```bash
   dotnet restore
   dotnet build
   dotnet run --project src/Api
   ```
6. **Run Tests**:
   ```bash
   dotnet test tests/UnitTests
   dotnet test tests/IntegrationTests
   ```
7. **Test APIs**:
   - Use REST Client extension to send requests to `/api/scheduling/appointments` or `/api/billing/invoices`.

## Project Structure
```
ThatInterpretingAgency/
├── src/
│   ├── Core/                   # Domain and Application logic
│   │   ├── Domain/            # DDD entities, aggregates
│   │   ├── Application/       # CQRS commands/queries
│   ├── Infrastructure/         # EF Core, QuickBooks integration
│   │   ├── Persistence/       # MSSQL DbContext, migrations
│   │   ├── Services/          # QuickBooksService, EmailService
│   ├── Api/                   # ASP.NET Core API
│   ├── WorkerServices/        # Background tasks (e.g., notifications)
├── tests/
│   ├── UnitTests/            # xUnit tests for domain logic
│   ├── IntegrationTests/     # Tests for MSSQL, QuickBooks
│   ├── EndToEndTests/        # API and flow tests
├── README.md
```

## Next Steps
- Implement vertical slices for each user story (e.g., Billing slice with QuickBooks).
- Set up CI/CD with GitHub Actions for automated testing and deployment.
- Monitor QuickBooks API rate limits and implement retry logic.

For contributions or issues, please submit a pull request or open an issue on GitHub.