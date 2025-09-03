# Calendar Integration Guide for THAT Interpreting Agency

## Overview
This guide explains the comprehensive calendar integration system that allows the agency to integrate with third-party calendars (Outlook, Gmail, iCal) while maintaining a standalone calendar system.

## Architecture Overview

### **Database Schema**
The calendar integration consists of 7 main tables:

1. **CalendarProviders** - Third-party calendar services (Outlook, Gmail, iCal)
2. **UserCalendarConnections** - User's connected external calendars
3. **CalendarEvents** - All calendar events (appointments, availability, blocked time)
4. **CalendarEventAttendees** - Event participants and responses
5. **CalendarSyncLogs** - Tracking of sync operations
6. **CalendarTemplates** - Recurring availability patterns
7. **CalendarTemplateRules** - Template time slot definitions

### **Integration Flow**
```
User Calendar ←→ THAT Agency System ←→ Third-party Calendars
                    ↓
            AvailabilitySlots (existing)
                    ↓
            Appointments (existing)
```

## Key Features

### 1. **Standalone Calendar System**
- Complete calendar functionality independent of external services
- Event management, scheduling, and availability tracking
- Template-based recurring availability patterns

### 2. **Third-party Calendar Integration**
- **Microsoft Outlook**: Graph API integration
- **Google Calendar**: Google Calendar API v3
- **Apple iCal**: iCal file import/export
- **Custom Calendar**: Webhook-based integration

### 3. **Bidirectional Sync**
- Pull events from external calendars
- Push agency events to external calendars
- Conflict resolution and duplicate prevention
- Real-time sync status tracking

### 4. **Availability Management**
- Convert existing AvailabilitySlots to CalendarEvents
- Template-based availability scheduling
- Conflict detection and resolution
- Time zone support

## Database Tables Structure

### **CalendarProviders**
```sql
CREATE TABLE [dbo].[CalendarProviders] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(100) NOT NULL,           -- 'Microsoft Outlook', 'Google Calendar'
    [ProviderType] nvarchar(50) NOT NULL,    -- 'Outlook', 'Gmail', 'iCal', 'Custom'
    [ApiEndpoint] nvarchar(500) NULL,        -- API endpoint URL
    [ClientId] nvarchar(200) NULL,           -- OAuth client ID
    [ClientSecret] nvarchar(500) NULL,       -- OAuth client secret
    [Scopes] nvarchar(500) NULL,             -- Required permissions
    [IsActive] bit NOT NULL DEFAULT 1,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL
);
```

### **UserCalendarConnections**
```sql
CREATE TABLE [dbo].[UserCalendarConnections] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] nvarchar(450) NOT NULL,         -- Links to AspNetUsers
    [ProviderId] uniqueidentifier NOT NULL,  -- Links to CalendarProviders
    [ExternalCalendarId] nvarchar(200) NULL, -- External calendar ID
    [CalendarName] nvarchar(200) NOT NULL,   -- User-friendly name
    [AccessToken] nvarchar(max) NULL,        -- OAuth access token
    [RefreshToken] nvarchar(max) NULL,       -- OAuth refresh token
    [TokenExpiresAt] datetime2 NULL,         -- Token expiration
    [IsActive] bit NOT NULL DEFAULT 1,
    [SyncEnabled] bit NOT NULL DEFAULT 1,
    [LastSyncAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL
);
```

### **CalendarEvents**
```sql
CREATE TABLE [dbo].[CalendarEvents] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] nvarchar(450) NOT NULL,         -- Event owner
    [ConnectionId] uniqueidentifier NULL,    -- External calendar connection
    [ExternalEventId] nvarchar(200) NULL,   -- External event ID
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Location] nvarchar(500) NULL,
    [StartTimeUtc] datetime2 NOT NULL,
    [EndTimeUtc] datetime2 NOT NULL,
    [TimeZone] nvarchar(50) NOT NULL,
    [EventType] nvarchar(50) NOT NULL,      -- 'Appointment', 'Availability', 'Blocked'
    [Status] nvarchar(50) NOT NULL,         -- 'Active', 'Cancelled', 'Completed'
    [IsAllDay] bit NOT NULL DEFAULT 0,
    [RecurrenceRule] nvarchar(500) NULL,    -- RRULE format
    [RecurrenceException] nvarchar(max) NULL, -- Exception dates
    [Color] nvarchar(7) NULL,               -- Hex color
    [Priority] int NOT NULL DEFAULT 0,
    [ReminderMinutes] int NULL,             -- Reminder timing
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL
);
```

## Integration with Existing System

### **Migration from AvailabilitySlots**
The new calendar system integrates with your existing AvailabilitySlots:

1. **AvailabilitySlots** remain for backward compatibility
2. **CalendarEvents** with EventType='Availability' provide enhanced functionality
3. **Bidirectional sync** keeps both systems in sync

### **Appointment Integration**
- Existing appointments can be represented as CalendarEvents
- External calendar events can create local appointments
- Conflict detection prevents double-booking

## Third-party Calendar Integration

### **Microsoft Outlook Integration**
```csharp
// OAuth 2.0 flow
var outlookProvider = await _calendarService.GetProviderByTypeAsync("Outlook");
var connection = await _calendarService.CreateConnectionAsync(
    userId, 
    outlookProvider.Id, 
    "Work Calendar"
);

// Graph API calls
await _calendarService.SyncWithExternalCalendarAsync(connection.Id);
```

### **Google Calendar Integration**
```csharp
// Google OAuth 2.0
var gmailProvider = await _calendarService.GetProviderByTypeAsync("Gmail");
var connection = await _calendarService.CreateConnectionAsync(
    userId, 
    gmailProvider.Id, 
    "Personal Calendar"
);

// Google Calendar API
await _calendarService.PullEventsFromExternalCalendarAsync(
    connection.Id, 
    startTime, 
    endTime
);
```

### **iCal Integration**
```csharp
// Import iCal file
var icalEvents = await _icalService.ParseICalFileAsync(icalContent);
foreach (var icalEvent in icalEvents)
{
    var calendarEvent = CalendarEvent.Create(
        userId,
        icalEvent.Summary,
        icalEvent.StartTime,
        icalEvent.EndTime,
        icalEvent.TimeZone,
        "Custom"
    );
    await _calendarService.CreateEventAsync(calendarEvent);
}
```

## Usage Examples

### **Creating Availability Slots**
```csharp
// Create availability slot
var availabilityEvent = CalendarEvent.Create(
    interpreterId,
    "Available for Interpreting",
    startTime,
    endTime,
    "America/New_York",
    "Availability"
);

await _calendarService.CreateEventAsync(availabilityEvent);

// Sync to external calendar
if (connection != null)
{
    await _calendarService.PushEventToExternalCalendarAsync(availabilityEvent.Id);
}
```

### **Checking Availability**
```csharp
// Check if time slot is available
var isAvailable = await _calendarService.IsTimeSlotAvailableAsync(
    interpreterId, 
    requestedStart, 
    requestedEnd
);

if (isAvailable)
{
    // Create appointment
    var appointment = CalendarEvent.Create(
        interpreterId,
        "Interpreting Session",
        requestedStart,
        requestedEnd,
        "America/New_York",
        "Appointment"
    );
    
    await _calendarService.CreateEventAsync(appointment);
}
```

### **Template-based Availability**
```csharp
// Create weekly availability template
var template = new CalendarTemplate
{
    Name = "Standard Work Week",
    TemplateType = "Weekly",
    Rules = new List<CalendarTemplateRule>
    {
        new CalendarTemplateRule { DayOfWeek = 1, StartTime = "09:00", EndTime = "17:00" },
        new CalendarTemplateRule { DayOfWeek = 2, StartTime = "09:00", EndTime = "17:00" },
        new CalendarTemplateRule { DayOfWeek = 3, StartTime = "09:00", EndTime = "17:00" },
        new CalendarTemplateRule { DayOfWeek = 4, StartTime = "09:00", EndTime = "17:00" },
        new CalendarTemplateRule { DayOfWeek = 5, StartTime = "09:00", EndTime = "17:00" }
    }
};

// Generate events from template
var events = await _calendarService.GenerateEventsFromTemplateAsync(
    template.Id, 
    startDate, 
    endDate
);
```

## API Endpoints

### **Calendar Management**
- `GET /api/calendar/events` - Get user's calendar events
- `POST /api/calendar/events` - Create new calendar event
- `PUT /api/calendar/events/{id}` - Update calendar event
- `DELETE /api/calendar/events/{id}` - Delete calendar event

### **External Calendar Integration**
- `GET /api/calendar/providers` - Get available calendar providers
- `POST /api/calendar/connections` - Connect external calendar
- `POST /api/calendar/connections/{id}/sync` - Sync with external calendar
- `DELETE /api/calendar/connections/{id}` - Disconnect external calendar

### **Availability Management**
- `GET /api/calendar/availability` - Get available time slots
- `POST /api/calendar/availability` - Create availability slot
- `GET /api/calendar/conflicts` - Check for scheduling conflicts
- `POST /api/calendar/templates` - Create availability template

## Configuration

### **appsettings.json**
```json
{
  "CalendarIntegration": {
    "Outlook": {
      "ClientId": "your-outlook-client-id",
      "ClientSecret": "your-outlook-client-secret",
      "TenantId": "your-tenant-id"
    },
    "Google": {
      "ClientId": "your-google-client-id",
      "ClientSecret": "your-google-client-secret"
    },
    "SyncSettings": {
      "DefaultSyncInterval": "00:15:00",
      "MaxRetryAttempts": 3,
      "ConflictResolution": "PreferLocal"
    }
  }
}
```

### **Database Connection String**
Ensure your connection string includes the new calendar tables:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=ThatInterpretingAgency;...;"
  }
}
```

## Security Considerations

### **OAuth 2.0 Implementation**
- Secure token storage in database
- Token refresh handling
- Scope-limited permissions
- Secure callback URLs

### **Data Privacy**
- User consent for external calendar access
- Data encryption for sensitive information
- GDPR compliance for EU users
- Audit logging for all operations

## Performance Optimization

### **Database Indexes**
- Composite indexes on UserId + StartTime + EndTime
- Indexes on EventType and Status
- Foreign key indexes for connections

### **Caching Strategy**
- Redis cache for frequently accessed events
- In-memory cache for user preferences
- Database query optimization

### **Sync Performance**
- Incremental sync for large calendars
- Background job processing
- Rate limiting for external APIs

## Testing and Validation

### **Unit Tests**
- Calendar event creation and validation
- Conflict detection algorithms
- Template generation logic

### **Integration Tests**
- External calendar API integration
- Database operations and constraints
- Sync operation validation

### **End-to-End Tests**
- Complete calendar workflow
- External calendar synchronization
- Conflict resolution scenarios

## Deployment Checklist

### **Pre-deployment**
- [ ] Run database migration scripts
- [ ] Configure OAuth applications
- [ ] Set up external API credentials
- [ ] Test database connectivity

### **Post-deployment**
- [ ] Verify calendar tables created
- [ ] Test external calendar connections
- [ ] Validate sync operations
- [ ] Monitor error logs

## Troubleshooting

### **Common Issues**
1. **OAuth Token Expired**: Implement automatic token refresh
2. **Sync Conflicts**: Use conflict resolution strategies
3. **Rate Limiting**: Implement exponential backoff
4. **Data Inconsistency**: Use transaction-based operations

### **Monitoring**
- Sync operation logs
- API response times
- Error rates and types
- Database performance metrics

## Next Steps

1. **Run the database script** to create calendar tables
2. **Implement the calendar service** with basic functionality
3. **Add OAuth integration** for external calendars
4. **Create calendar UI components** in BlazorFrontend
5. **Test integration** with real external calendars
6. **Deploy and monitor** the system

## Support

For questions or issues with the calendar integration:
1. Check the database schema and constraints
2. Verify OAuth configuration
3. Review sync operation logs
4. Test with external calendar APIs
5. Consult the troubleshooting guide
