# PayPing Backend Documentation

## Overview
The PayPing backend is a robust ASP.NET Core Web API designed to manage financial reminders and automated notifications. It serves as the central hub for user authentication, reminder creation, payment history tracking, and automated WhatsApp alerting.

## Architecture
The system follows a **Clean Architecture** (or Onion Architecture) pattern, ensuring a clear separation of concerns and high maintainability:

1.  **PayPing.API**: The entry point. Handles HTTP requests, authentication middleware, and Swagger documentation.
2.  **PayPing.Application**: Core business logic. Contains DTOs (Data Transfer Objects) and service interfaces.
3.  **PayPing.Domain**: Pure domain models. Contains database entities ([Reminder](file:///d:/Dev/Internship/Xhire_Tech/Projects/PayPing/backend/PayPing.Infrastructure/Services/ReminderBackgroundService.cs#48-99), `AppUser`, `PaymentHistory`) and core exceptions.
4.  **PayPing.Infrastructure**: Implementation details. Handles database persistence (Entity Framework Core with PostgreSQL), external service integrations (WhatsApp), and background worker processes.

## Implementation Details

### Core Workflows
- **Authentication**: Utilizes ASP.NET Core Identity with PostgreSQL. JWT tokens are issued for secure API access.
- **Reminder Management**: Reminders are stored in the [Reminders](file:///d:/Dev/Internship/Xhire_Tech/Projects/PayPing/backend/PayPing.Infrastructure/Services/ReminderBackgroundService.cs#48-99) table. Users can set "One-time" or recurring (Daily, Weekly, Monthly, Yearly, or "Every X Days/Weeks/Months") frequencies.
- **Automated Alerts**: A [ReminderBackgroundService](file:///d:/Dev/Internship/Xhire_Tech/Projects/PayPing/backend/PayPing.Infrastructure/Services/ReminderBackgroundService.cs#20-25) runs every minute to:
    1. Query due reminders (based on `NextReminderDate`).
    2. Send WhatsApp messages via `WhatsAppService`.
    3. Calculate the next occurrence date for recurring reminders.
    4. Update the database state.

### Technology Stack
- **Framework**: .NET 8.0 / ASP.NET Core API
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core
- **Auth**: JWT Bearer + Microsoft Identity
- **Messaging**: WhatsApp API (integrated via `WhatsAppService`)

## Important Files

### API Layer
- [PayPing.API/Program.cs](file:///d:/Dev/Internship/Xhire_Tech/Projects/PayPing/backend/PayPing.API/Program.cs): Dependency injection, middleware pipeline, and main configuration.
- [PayPing.API/Controllers/RemindersController.cs](file:///d:/Dev/Internship/Xhire_Tech/Projects/PayPing/backend/PayPing.API/Controllers/RemindersController.cs): Main CRUD operations for reminders.
- [PayPing.API/Controllers/AuthController.cs](file:///d:/Dev/Internship/Xhire_Tech/Projects/PayPing/backend/PayPing.API/Controllers/AuthController.cs): Login and registration logic.

### Infrastructure Layer
- [PayPing.Infrastructure/Persistence/ApplicationDbContext.cs](file:///d:/Dev/Internship/Xhire_Tech/Projects/PayPing/backend/PayPing.Infrastructure/Persistence/ApplicationDbContext.cs): Database context and entity configurations.
- [PayPing.Infrastructure/Services/ReminderBackgroundService.cs](file:///d:/Dev/Internship/Xhire_Tech/Projects/PayPing/backend/PayPing.Infrastructure/Services/ReminderBackgroundService.cs): The core engine for processing automated alerts.
- [PayPing.Infrastructure/Services/WhatsAppService.cs](file:///d:/Dev/Internship/Xhire_Tech/Projects/PayPing/backend/PayPing.Infrastructure/Services/WhatsAppService.cs): Integration with the WhatsApp messaging platform.

## Example Usage

### Creating a Reminder (POST /api/reminders)
```json
{
  "name": "Cloud Subscription",
  "phoneNumber": "1234567890",
  "amount": 1500,
  "frequency": "Monthly",
  "reminderDate": "2024-04-01T10:00:00Z",
  "message": "Your cloud subscription is due tomorrow."
}
```

### Remainder Frequency Logic
The system supports flexible frequency strings like:
- `Daily`, `Weekly`, `Monthly`, `Yearly`
- `Every 3 Days`
- `Every 2 Months`

## Developer Notes

### Setting Up the Database
Ensure PostgreSQL is running and the connection string in [appsettings.json](file:///d:/Dev/Internship/Xhire_Tech/Projects/PayPing/backend/PayPing.API/appsettings.json) is correct. Use EF Core migrations to update the schema:
```bash
dotnet ef database update --project PayPing.Infrastructure --startup-project PayPing.API
```

### Debugging Background Services
Logs for the [ReminderBackgroundService](file:///d:/Dev/Internship/Xhire_Tech/Projects/PayPing/backend/PayPing.Infrastructure/Services/ReminderBackgroundService.cs#20-25) can be found in the application output. It processes reminders in 1-minute intervals. If a reminder is not sending, check:
1. `IsPaid` status (should be false).
2. `NextReminderDate` (should be <= current time).
3. `EndDate` (should be null or > current time).
4. WhatsApp Service logs for API errors.

### Security
All endpoints except Login/Register are protected by the `[Authorize]` attribute. Ensure the `Authorization` header contains the `Bearer <token>` for all requests.
