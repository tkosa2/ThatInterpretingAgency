-- THAT Interpreting Agency Database Schema
-- This script creates the initial database schema for the application

USE [ThatInterpretingAgency]
GO

-- Create Agencies table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Agencies](
        [Id] [uniqueidentifier] NOT NULL,
        [Name] [nvarchar](100) NOT NULL,
        [ContactInfo] [nvarchar](500) NOT NULL,
        [Status] [nvarchar](20) NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_Agencies] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create unique index on Agency Name
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Agencies_Name')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Agencies_Name] ON [dbo].[Agencies]([Name] ASC)
END
GO

-- Create AgencyStaff table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AgencyStaff]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AgencyStaff](
        [Id] [uniqueidentifier] NOT NULL,
        [AgencyId] [uniqueidentifier] NOT NULL,
        [UserId] [nvarchar](450) NOT NULL,
        [Role] [nvarchar](50) NOT NULL,
        [HireDate] [datetime2](7) NOT NULL,
        [Status] [nvarchar](20) NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_AgencyStaff] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create unique index on AgencyId, UserId, Role combination
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AgencyStaff_AgencyId_UserId_Role')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_AgencyStaff_AgencyId_UserId_Role] ON [dbo].[AgencyStaff]([AgencyId] ASC, [UserId] ASC, [Role] ASC)
END
GO

-- Create Interpreters table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Interpreters]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Interpreters](
        [Id] [uniqueidentifier] NOT NULL,
        [AgencyId] [uniqueidentifier] NOT NULL,
        [UserId] [nvarchar](450) NOT NULL,
        [FullName] [nvarchar](200) NOT NULL,
        [Skills] [nvarchar](max) NOT NULL,
        [Status] [nvarchar](20) NOT NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_Interpreters] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create unique index on AgencyId, UserId combination
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Interpreters_AgencyId_UserId')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Interpreters_AgencyId_UserId] ON [dbo].[Interpreters]([AgencyId] ASC, [UserId] ASC)
END
GO

-- Create Clients table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Clients](
        [Id] [uniqueidentifier] NOT NULL,
        [AgencyId] [uniqueidentifier] NOT NULL,
        [UserId] [nvarchar](450) NOT NULL,
        [OrganizationName] [nvarchar](200) NOT NULL,
        [Preferences] [nvarchar](max) NULL,
        [Status] [nvarchar](20) NOT NULL,
        [ContactPerson] [nvarchar](200) NULL,
        [PhoneNumber] [nvarchar](20) NULL,
        [Email] [nvarchar](100) NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_Clients] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create unique index on AgencyId, UserId combination
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Clients_AgencyId_UserId')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Clients_AgencyId_UserId] ON [dbo].[Clients]([AgencyId] ASC, [UserId] ASC)
END
GO

-- Create Appointments table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Appointments](
        [Id] [uniqueidentifier] NOT NULL,
        [AgencyId] [uniqueidentifier] NOT NULL,
        [InterpreterId] [uniqueidentifier] NOT NULL,
        [ClientId] [uniqueidentifier] NOT NULL,
        [StartTime] [datetime2](7) NOT NULL,
        [EndTime] [datetime2](7) NOT NULL,
        [TimeZone] [nvarchar](50) NOT NULL,
        [Status] [nvarchar](20) NOT NULL,
        [Notes] [nvarchar](1000) NULL,
        [Location] [nvarchar](500) NULL,
        [Language] [nvarchar](50) NULL,
        [Rate] [decimal](18, 2) NULL,
        [CancellationReason] [nvarchar](500) NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_Appointments] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create indexes on Appointments table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_AgencyId_InterpreterId_StartTime')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Appointments_AgencyId_InterpreterId_StartTime] ON [dbo].[Appointments]([AgencyId] ASC, [InterpreterId] ASC, [StartTime] ASC)
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Appointments_AgencyId_ClientId_StartTime')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Appointments_AgencyId_ClientId_StartTime] ON [dbo].[Appointments]([AgencyId] ASC, [ClientId] ASC, [StartTime] ASC)
END
GO

-- Create Invoices table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Invoices]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Invoices](
        [Id] [uniqueidentifier] NOT NULL,
        [AgencyId] [uniqueidentifier] NOT NULL,
        [ClientId] [uniqueidentifier] NOT NULL,
        [AppointmentId] [uniqueidentifier] NOT NULL,
        [QuickBooksInvoiceId] [nvarchar](50) NOT NULL,
        [Status] [nvarchar](20) NOT NULL,
        [DueDate] [datetime2](7) NULL,
        [Amount] [decimal](18, 2) NULL,
        [Currency] [nvarchar](3) NULL,
        [Notes] [nvarchar](1000) NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create unique index on QuickBooksInvoiceId per agency
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_AgencyId_QuickBooksInvoiceId')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Invoices_AgencyId_QuickBooksInvoiceId] ON [dbo].[Invoices]([AgencyId] ASC, [QuickBooksInvoiceId] ASC)
END
GO

-- Create index on AgencyId, ClientId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Invoices_AgencyId_ClientId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Invoices_AgencyId_ClientId] ON [dbo].[Invoices]([AgencyId] ASC, [ClientId] ASC)
END
GO

-- Create Notifications table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Notifications](
        [Id] [uniqueidentifier] NOT NULL,
        [AgencyId] [uniqueidentifier] NOT NULL,
        [UserId] [nvarchar](450) NOT NULL,
        [Type] [nvarchar](20) NOT NULL,
        [Message] [nvarchar](1000) NOT NULL,
        [Status] [nvarchar](20) NOT NULL,
        [SentAt] [datetime2](7) NULL,
        [ReadAt] [datetime2](7) NULL,
        [ExternalReference] [nvarchar](100) NULL,
        [Metadata] [nvarchar](max) NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_Notifications] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create index on Notifications table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Notifications_AgencyId_UserId_Status')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Notifications_AgencyId_UserId_Status] ON [dbo].[Notifications]([AgencyId] ASC, [UserId] ASC, [Status] ASC)
END
GO

-- Create AvailabilitySlots table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AvailabilitySlots]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[AvailabilitySlots](
        [Id] [uniqueidentifier] NOT NULL,
        [StartTime] [datetime2](7) NOT NULL,
        [EndTime] [datetime2](7) NOT NULL,
        [TimeZone] [nvarchar](50) NOT NULL,
        [Status] [nvarchar](20) NOT NULL,
        [Notes] [nvarchar](500) NULL,
        [CreatedAt] [datetime2](7) NOT NULL,
        [UpdatedAt] [datetime2](7) NOT NULL,
        CONSTRAINT [PK_AvailabilitySlots] PRIMARY KEY CLUSTERED ([Id] ASC)
    )
END
GO

-- Create index on AvailabilitySlots table
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_AvailabilitySlots_StartTime_EndTime')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AvailabilitySlots_StartTime_EndTime] ON [dbo].[AvailabilitySlots]([StartTime] ASC, [EndTime] ASC)
END
GO

-- Add foreign key constraints
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_AgencyStaff_Agencies')
BEGIN
    ALTER TABLE [dbo].[AgencyStaff] ADD CONSTRAINT [FK_AgencyStaff_Agencies] 
        FOREIGN KEY([AgencyId]) REFERENCES [dbo].[Agencies] ([Id]) ON DELETE CASCADE
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Interpreters_Agencies')
BEGIN
    ALTER TABLE [dbo].[Interpreters] ADD CONSTRAINT [FK_Interpreters_Agencies] 
        FOREIGN KEY([AgencyId]) REFERENCES [dbo].[Agencies] ([Id]) ON DELETE CASCADE
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Clients_Agencies')
BEGIN
    ALTER TABLE [dbo].[Clients] ADD CONSTRAINT [FK_Clients_Agencies] 
        FOREIGN KEY([AgencyId]) REFERENCES [dbo].[Agencies] ([Id]) ON DELETE CASCADE
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Agencies')
BEGIN
    ALTER TABLE [dbo].[Appointments] ADD CONSTRAINT [FK_Appointments_Agencies] 
        FOREIGN KEY([AgencyId]) REFERENCES [dbo].[Agencies] ([Id]) ON DELETE CASCADE
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Interpreters')
BEGIN
    ALTER TABLE [dbo].[Appointments] ADD CONSTRAINT [FK_Appointments_Interpreters] 
        FOREIGN KEY([InterpreterId]) REFERENCES [dbo].[Interpreters] ([Id]) ON DELETE CASCADE
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Appointments_Clients')
BEGIN
    ALTER TABLE [dbo].[Appointments] ADD CONSTRAINT [FK_Appointments_Clients] 
        FOREIGN KEY([ClientId]) REFERENCES [dbo].[Clients] ([Id]) ON DELETE CASCADE
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Invoices_Agencies')
BEGIN
    ALTER TABLE [dbo].[Invoices] ADD CONSTRAINT [FK_Invoices_Agencies] 
        FOREIGN KEY([AgencyId]) REFERENCES [dbo].[Agencies] ([Id]) ON DELETE CASCADE
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Invoices_Clients')
BEGIN
    ALTER TABLE [dbo].[Invoices] ADD CONSTRAINT [FK_Invoices_Clients] 
        FOREIGN KEY([ClientId]) REFERENCES [dbo].[Clients] ([Id]) ON DELETE CASCADE
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Invoices_Appointments')
BEGIN
    ALTER TABLE [dbo].[Invoices] ADD CONSTRAINT [FK_Invoices_Appointments] 
        FOREIGN KEY([AppointmentId]) REFERENCES [dbo].[Appointments] ([Id]) ON DELETE CASCADE
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Notifications_Agencies')
BEGIN
    ALTER TABLE [dbo].[Notifications] ADD CONSTRAINT [FK_Notifications_Agencies] 
        FOREIGN KEY([AgencyId]) REFERENCES [dbo].[Agencies] ([Id]) ON DELETE CASCADE
END
GO

-- Insert default agency
IF NOT EXISTS (SELECT * FROM [dbo].[Agencies] WHERE [Name] = 'THAT Interpreting Agency')
BEGIN
    INSERT INTO [dbo].[Agencies] ([Id], [Name], [ContactInfo], [Status], [CreatedAt], [UpdatedAt])
    VALUES (
        NEWID(),
        'THAT Interpreting Agency',
        'Global interpreting services for all languages and locations',
        'Active',
        GETUTCDATE(),
        GETUTCDATE()
    )
END
GO

PRINT 'Database schema created successfully!'
PRINT 'Default agency "THAT Interpreting Agency" has been created.'
GO
