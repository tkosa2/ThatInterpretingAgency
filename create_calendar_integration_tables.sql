-- Create Calendar Integration Tables for THAT Interpreting Agency
-- This script adds calendar functionality that integrates with third-party calendars (Outlook, Gmail, etc.)
-- 
-- Note: Foreign key constraints to AspNetUsers use ON DELETE NO ACTION to prevent cascade path issues.
-- When a user is deleted, related calendar data should be handled by application logic or triggers.

USE [ThatInterpretingAgency];
GO

PRINT 'Starting Calendar Integration Tables creation...';
GO

-- 1. Create CalendarProviders table for third-party calendar services
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CalendarProviders')
BEGIN
    CREATE TABLE [dbo].[CalendarProviders] (
        [Id] uniqueidentifier NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [ProviderType] nvarchar(50) NOT NULL, -- 'Outlook', 'Gmail', 'iCal', 'Custom'
        [ApiEndpoint] nvarchar(500) NULL,
        [ClientId] nvarchar(200) NULL,
        [ClientSecret] nvarchar(500) NULL,
        [Scopes] nvarchar(500) NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CalendarProviders] PRIMARY KEY ([Id])
    );
    
    PRINT 'Created CalendarProviders table';
    
    -- Insert default providers
    INSERT INTO [dbo].[CalendarProviders] ([Id], [Name], [ProviderType], [ApiEndpoint], [IsActive], [CreatedAt], [UpdatedAt])
    VALUES 
        (NEWID(), 'Microsoft Outlook', 'Outlook', 'https://graph.microsoft.com/v1.0', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Google Calendar', 'Gmail', 'https://www.googleapis.com/calendar/v3', 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Apple iCal', 'iCal', NULL, 1, GETUTCDATE(), GETUTCDATE()),
        (NEWID(), 'Custom Calendar', 'Custom', NULL, 1, GETUTCDATE(), GETUTCDATE());
    
    PRINT 'Inserted default calendar providers';
END
ELSE
BEGIN
    PRINT 'CalendarProviders table already exists';
END

-- 2. Create UserCalendarConnections table for user's connected calendars
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'UserCalendarConnections')
BEGIN
    CREATE TABLE [dbo].[UserCalendarConnections] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [ProviderId] uniqueidentifier NOT NULL,
        [ExternalCalendarId] nvarchar(200) NULL,
        [CalendarName] nvarchar(200) NOT NULL,
        [AccessToken] nvarchar(max) NULL,
        [RefreshToken] nvarchar(max) NULL,
        [TokenExpiresAt] datetime2 NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [SyncEnabled] bit NOT NULL DEFAULT 1,
        [LastSyncAt] datetime2 NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_UserCalendarConnections] PRIMARY KEY ([Id])
    );
    
    PRINT 'Created UserCalendarConnections table';
    
    -- Create indexes for performance
    CREATE INDEX [IX_UserCalendarConnections_UserId] ON [dbo].[UserCalendarConnections] ([UserId]);
    CREATE INDEX [IX_UserCalendarConnections_ProviderId] ON [dbo].[UserCalendarConnections] ([ProviderId]);
    CREATE INDEX [IX_UserCalendarConnections_ExternalCalendarId] ON [dbo].[UserCalendarConnections] ([ExternalCalendarId]);
    
    PRINT 'Created performance indexes for UserCalendarConnections table';
END
ELSE
BEGIN
    PRINT 'UserCalendarConnections table already exists';
END

-- 3. Create CalendarEvents table for all calendar events
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CalendarEvents')
BEGIN
    CREATE TABLE [dbo].[CalendarEvents] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [ConnectionId] uniqueidentifier NULL,
        [ExternalEventId] nvarchar(200) NULL,
        [Title] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [Location] nvarchar(500) NULL,
        [StartTimeUtc] datetime2 NOT NULL,
        [EndTimeUtc] datetime2 NOT NULL,
        [TimeZone] nvarchar(50) NOT NULL,
        [EventType] nvarchar(50) NOT NULL, -- 'Appointment', 'Availability', 'Blocked', 'Custom'
        [Status] nvarchar(50) NOT NULL DEFAULT 'Active', -- 'Active', 'Cancelled', 'Completed'
        [IsAllDay] bit NOT NULL DEFAULT 0,
        [RecurrenceRule] nvarchar(500) NULL, -- RRULE format for recurring events
        [RecurrenceException] nvarchar(max) NULL, -- JSON array of exception dates
        [Color] nvarchar(7) NULL, -- Hex color code
        [Priority] int NOT NULL DEFAULT 0,
        [ReminderMinutes] int NULL, -- Minutes before event to send reminder
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CalendarEvents] PRIMARY KEY ([Id])
    );
    
    PRINT 'Created CalendarEvents table';
    
    -- Create indexes for performance
    CREATE INDEX [IX_CalendarEvents_UserId] ON [dbo].[CalendarEvents] ([UserId]);
    CREATE INDEX [IX_CalendarEvents_ConnectionId] ON [dbo].[CalendarEvents] ([ConnectionId]);
    CREATE INDEX [IX_CalendarEvents_StartTimeUtc] ON [dbo].[CalendarEvents] ([StartTimeUtc]);
    CREATE INDEX [IX_CalendarEvents_EndTimeUtc] ON [dbo].[CalendarEvents] ([EndTimeUtc]);
    CREATE INDEX [IX_CalendarEvents_EventType] ON [dbo].[CalendarEvents] ([EventType]);
    CREATE INDEX [IX_CalendarEvents_Status] ON [dbo].[CalendarEvents] ([Status]);
    CREATE INDEX [IX_CalendarEvents_UserId_StartTime_EndTime] ON [dbo].[CalendarEvents] ([UserId], [StartTimeUtc], [EndTimeUtc]);
    
    PRINT 'Created performance indexes for CalendarEvents table';
END
ELSE
BEGIN
    PRINT 'CalendarEvents table already exists';
END

-- 4. Create CalendarEventAttendees table for event participants
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CalendarEventAttendees')
BEGIN
    CREATE TABLE [dbo].[CalendarEventAttendees] (
        [Id] uniqueidentifier NOT NULL,
        [EventId] uniqueidentifier NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [Name] nvarchar(200) NULL,
        [ResponseStatus] nvarchar(50) NOT NULL DEFAULT 'Pending', -- 'Pending', 'Accepted', 'Declined', 'Tentative'
        [ResponseTime] datetime2 NULL,
        [IsOrganizer] bit NOT NULL DEFAULT 0,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CalendarEventAttendees] PRIMARY KEY ([Id])
    );
    
    PRINT 'Created CalendarEventAttendees table';
    
    -- Create indexes for performance
    CREATE INDEX [IX_CalendarEventAttendees_EventId] ON [dbo].[CalendarEventAttendees] ([EventId]);
    CREATE INDEX [IX_CalendarEventAttendees_Email] ON [dbo].[CalendarEventAttendees] ([Email]);
    CREATE INDEX [IX_CalendarEventAttendees_ResponseStatus] ON [dbo].[CalendarEventAttendees] ([ResponseStatus]);
    
    PRINT 'Created performance indexes for CalendarEventAttendees table';
END
ELSE
BEGIN
    PRINT 'CalendarEventAttendees table already exists';
END

-- 5. Create CalendarSyncLogs table for tracking sync operations
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CalendarSyncLogs')
BEGIN
    CREATE TABLE [dbo].[CalendarSyncLogs] (
        [Id] uniqueidentifier NOT NULL,
        [ConnectionId] uniqueidentifier NOT NULL,
        [SyncType] nvarchar(50) NOT NULL, -- 'Full', 'Incremental', 'Manual'
        [Status] nvarchar(50) NOT NULL, -- 'Success', 'Failed', 'Partial'
        [StartTime] datetime2 NOT NULL,
        [EndTime] datetime2 NULL,
        [EventsProcessed] int NOT NULL DEFAULT 0,
        [EventsCreated] int NOT NULL DEFAULT 0,
        [EventsUpdated] int NOT NULL DEFAULT 0,
        [EventsDeleted] int NOT NULL DEFAULT 0,
        [ErrorMessage] nvarchar(max) NULL,
        [CreatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CalendarSyncLogs] PRIMARY KEY ([Id])
    );
    
    PRINT 'Created CalendarSyncLogs table';
    
    -- Create indexes for performance
    CREATE INDEX [IX_CalendarSyncLogs_ConnectionId] ON [dbo].[CalendarSyncLogs] ([ConnectionId]);
    CREATE INDEX [IX_CalendarSyncLogs_StartTime] ON [dbo].[CalendarSyncLogs] ([StartTime]);
    CREATE INDEX [IX_CalendarSyncLogs_Status] ON [dbo].[CalendarSyncLogs] ([Status]);
    
    PRINT 'Created performance indexes for CalendarSyncLogs table';
END
ELSE
BEGIN
    PRINT 'CalendarSyncLogs table already exists';
END

-- 6. Create CalendarTemplates table for recurring availability patterns
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CalendarTemplates')
BEGIN
    CREATE TABLE [dbo].[CalendarTemplates] (
        [Id] uniqueidentifier NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [Name] nvarchar(200) NOT NULL,
        [Description] nvarchar(max) NULL,
        [TemplateType] nvarchar(50) NOT NULL, -- 'Weekly', 'Monthly', 'Custom'
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CalendarTemplates] PRIMARY KEY ([Id])
    );
    
    PRINT 'Created CalendarTemplates table';
    
    -- Create indexes for performance
    CREATE INDEX [IX_CalendarTemplates_UserId] ON [dbo].[CalendarTemplates] ([UserId]);
    CREATE INDEX [IX_CalendarTemplates_TemplateType] ON [dbo].[CalendarTemplates] ([TemplateType]);
    
    PRINT 'Created performance indexes for CalendarTemplates table';
END
ELSE
BEGIN
    PRINT 'CalendarTemplates table already exists';
END

-- 7. Create CalendarTemplateRules table for template time slots
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CalendarTemplateRules')
BEGIN
    CREATE TABLE [dbo].[CalendarTemplateRules] (
        [Id] uniqueidentifier NOT NULL,
        [TemplateId] uniqueidentifier NOT NULL,
        [DayOfWeek] int NULL, -- 0=Sunday, 1=Monday, etc.
        [StartTime] time NOT NULL,
        [EndTime] time NOT NULL,
        [TimeZone] nvarchar(50) NOT NULL,
        [EventType] nvarchar(50) NOT NULL DEFAULT 'Availability',
        [Priority] int NOT NULL DEFAULT 0,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_CalendarTemplateRules] PRIMARY KEY ([Id])
    );
    
    PRINT 'Created CalendarTemplateRules table';
    
    -- Create indexes for performance
    CREATE INDEX [IX_CalendarTemplateRules_TemplateId] ON [dbo].[CalendarTemplateRules] ([TemplateId]);
    CREATE INDEX [IX_CalendarTemplateRules_DayOfWeek] ON [dbo].[CalendarTemplateRules] ([DayOfWeek]);
    CREATE INDEX [IX_CalendarTemplateRules_StartTime] ON [dbo].[CalendarTemplateRules] ([StartTime]);
    
    PRINT 'Created performance indexes for CalendarTemplateRules table';
END
ELSE
BEGIN
    PRINT 'CalendarTemplateRules table already exists';
END

-- 8. Add foreign key constraints
PRINT 'Adding foreign key constraints...';

-- UserCalendarConnections foreign keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_UserCalendarConnections_CalendarProviders_ProviderId')
BEGIN
    ALTER TABLE [dbo].[UserCalendarConnections] 
    ADD CONSTRAINT [FK_UserCalendarConnections_CalendarProviders_ProviderId] 
    FOREIGN KEY ([ProviderId]) REFERENCES [dbo].[CalendarProviders] ([Id]) 
    ON DELETE CASCADE;
    PRINT 'Added FK_UserCalendarConnections_CalendarProviders_ProviderId';
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_UserCalendarConnections_AspNetUsers_UserId')
BEGIN
    ALTER TABLE [dbo].[UserCalendarConnections] 
    ADD CONSTRAINT [FK_UserCalendarConnections_AspNetUsers_UserId] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) 
    ON DELETE NO ACTION;
    PRINT 'Added FK_UserCalendarConnections_AspNetUsers_UserId';
END

-- CalendarEvents foreign keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_CalendarEvents_UserCalendarConnections_ConnectionId')
BEGIN
    ALTER TABLE [dbo].[CalendarEvents] 
    ADD CONSTRAINT [FK_CalendarEvents_UserCalendarConnections_ConnectionId] 
    FOREIGN KEY ([ConnectionId]) REFERENCES [dbo].[UserCalendarConnections] ([Id]) 
    ON DELETE SET NULL;
    PRINT 'Added FK_CalendarEvents_UserCalendarConnections_ConnectionId';
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_CalendarEvents_AspNetUsers_UserId')
BEGIN
    ALTER TABLE [dbo].[CalendarEvents] 
    ADD CONSTRAINT [FK_CalendarEvents_AspNetUsers_UserId] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) 
    ON DELETE NO ACTION;
    PRINT 'Added FK_CalendarEvents_AspNetUsers_UserId';
END

-- CalendarEventAttendees foreign keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_CalendarEventAttendees_CalendarEvents_EventId')
BEGIN
    ALTER TABLE [dbo].[CalendarEventAttendees] 
    ADD CONSTRAINT [FK_CalendarEventAttendees_CalendarEvents_EventId] 
    FOREIGN KEY ([EventId]) REFERENCES [dbo].[CalendarEvents] ([Id]) 
    ON DELETE CASCADE;
    PRINT 'Added FK_CalendarEventAttendees_CalendarEvents_EventId';
END

-- CalendarSyncLogs foreign keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_CalendarSyncLogs_UserCalendarConnections_ConnectionId')
BEGIN
    ALTER TABLE [dbo].[CalendarSyncLogs] 
    ADD CONSTRAINT [FK_CalendarSyncLogs_UserCalendarConnections_ConnectionId] 
    FOREIGN KEY ([ConnectionId]) REFERENCES [dbo].[UserCalendarConnections] ([Id]) 
    ON DELETE CASCADE;
    PRINT 'Added FK_CalendarSyncLogs_UserCalendarConnections_ConnectionId';
END

-- CalendarTemplates foreign keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_CalendarTemplates_AspNetUsers_UserId')
BEGIN
    ALTER TABLE [dbo].[CalendarTemplates] 
    ADD CONSTRAINT [FK_CalendarTemplates_AspNetUsers_UserId] 
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) 
    ON DELETE NO ACTION;
    PRINT 'Added FK_CalendarTemplates_AspNetUsers_UserId';
END

-- CalendarTemplateRules foreign keys
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_CalendarTemplateRules_CalendarTemplates_TemplateId')
BEGIN
    ALTER TABLE [dbo].[CalendarTemplateRules] 
    ADD CONSTRAINT [FK_CalendarTemplateRules_CalendarTemplates_TemplateId] 
    FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[CalendarTemplates] ([Id]) 
    ON DELETE CASCADE;
    PRINT 'Added FK_CalendarTemplateRules_CalendarTemplates_TemplateId';
END

-- 9. Create views for easier querying
PRINT 'Creating calendar views...';

-- View for user's complete calendar
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_UserCalendar')
    DROP VIEW [dbo].[vw_UserCalendar];
GO

CREATE VIEW [dbo].[vw_UserCalendar] AS
SELECT 
    ce.Id,
    ce.UserId,
    u.Email AS UserEmail,
    ce.Title,
    ce.Description,
    ce.Location,
    ce.StartTimeUtc,
    ce.EndTimeUtc,
    ce.TimeZone,
    ce.EventType,
    ce.Status,
    ce.IsAllDay,
    ce.Color,
    ce.Priority,
    ucc.CalendarName,
    cp.Name AS ProviderName,
    cp.ProviderType
FROM [dbo].[CalendarEvents] ce
INNER JOIN [dbo].[AspNetUsers] u ON ce.UserId = u.Id
LEFT JOIN [dbo].[UserCalendarConnections] ucc ON ce.ConnectionId = ucc.Id
LEFT JOIN [dbo].[CalendarProviders] cp ON ucc.ProviderId = cp.Id
WHERE ce.Status = 'Active';
GO

PRINT 'Created vw_UserCalendar view';

-- View for availability conflicts
IF EXISTS (SELECT * FROM sys.views WHERE name = 'vw_AvailabilityConflicts')
    DROP VIEW [dbo].[vw_AvailabilityConflicts];
GO

CREATE VIEW [dbo].[vw_AvailabilityConflicts] AS
SELECT 
    ce1.Id AS Event1Id,
    ce1.Title AS Event1Title,
    ce1.StartTimeUtc AS Event1Start,
    ce1.EndTimeUtc AS Event1End,
    ce1.UserId AS User1Id,
    ce2.Id AS Event2Id,
    ce2.Title AS Event2Title,
    ce2.StartTimeUtc AS Event2Start,
    ce2.EndTimeUtc AS Event2End,
    ce2.UserId AS User2Id
FROM [dbo].[CalendarEvents] ce1
INNER JOIN [dbo].[CalendarEvents] ce2 ON 
    ce1.Id != ce2.Id AND
    ce1.UserId = ce2.UserId AND
    ce1.Status = 'Active' AND
    ce2.Status = 'Active' AND
    ce1.StartTimeUtc < ce2.EndTimeUtc AND
    ce1.EndTimeUtc > ce2.StartTimeUtc
WHERE ce1.EventType IN ('Appointment', 'Availability') AND ce2.EventType IN ('Appointment', 'Availability');
GO

PRINT 'Created vw_AvailabilityConflicts view';

-- 10. Verify the new structure
PRINT 'Verifying new table structure...';

SELECT 
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN (
    'CalendarProviders', 
    'UserCalendarConnections', 
    'CalendarEvents', 
    'CalendarEventAttendees', 
    'CalendarSyncLogs', 
    'CalendarTemplates', 
    'CalendarTemplateRules'
)
ORDER BY TABLE_NAME;

PRINT 'Calendar Integration Tables setup complete!';
GO

-- 11. Create sample data for testing
PRINT 'Creating sample calendar data...';

-- Insert sample calendar events for testing
IF NOT EXISTS (SELECT * FROM [dbo].[CalendarEvents] WHERE Title = 'Sample Interpreter Availability')
BEGIN
    INSERT INTO [dbo].[CalendarEvents] (
        [Id], [UserId], [Title], [Description], [StartTimeUtc], [EndTimeUtc], 
        [TimeZone], [EventType], [Status], [CreatedAt], [UpdatedAt]
    )
    SELECT TOP 1
        NEWID(),
        u.Id,
        'Sample Interpreter Availability',
        'Sample availability slot for testing',
        DATEADD(day, 1, GETUTCDATE()),
        DATEADD(day, 1, DATEADD(hour, 2, GETUTCDATE())),
        'UTC',
        'Availability',
        'Active',
        GETUTCDATE(),
        GETUTCDATE()
    FROM [dbo].[AspNetUsers] u
    WHERE u.Email LIKE '%@%';
    
    PRINT 'Created sample calendar event';
END

PRINT 'Calendar integration setup complete!';
GO

-- 12. Create trigger to handle user deletion cleanup
PRINT 'Creating user deletion cleanup trigger...';

IF EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_UserCalendarCleanup')
    DROP TRIGGER [dbo].[TR_UserCalendarCleanup];
GO

CREATE TRIGGER [dbo].[TR_UserCalendarCleanup]
ON [dbo].[AspNetUsers]
AFTER DELETE
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Clean up calendar events for deleted users
    DELETE FROM [dbo].[CalendarEvents] 
    WHERE [UserId] IN (SELECT [Id] FROM DELETED);
    
    -- Clean up user calendar connections for deleted users
    DELETE FROM [dbo].[UserCalendarConnections] 
    WHERE [UserId] IN (SELECT [Id] FROM DELETED);
    
    -- Clean up calendar templates for deleted users
    DELETE FROM [dbo].[CalendarTemplates] 
    WHERE [UserId] IN (SELECT [Id] FROM DELETED);
    
    PRINT 'Cleaned up calendar data for deleted users';
END
GO

PRINT 'Created user deletion cleanup trigger';
GO
