-- Fix missing columns in database to match Entity Framework model
-- This script adds the columns that EF expects but are missing from the database

USE [ThatInterpretingAgency];
GO

PRINT 'Starting database column fixes...';
GO

-- 1. Fix Interpreters table
PRINT 'Fixing Interpreters table...';

-- Add Availability column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Interpreters' AND COLUMN_NAME = 'Availability')
BEGIN
    ALTER TABLE [dbo].[Interpreters] 
    ADD [Availability] NVARCHAR(MAX) NOT NULL DEFAULT '[]';
    PRINT 'Added Availability column to Interpreters table';
END

-- Add Skills column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Interpreters' AND COLUMN_NAME = 'Skills')
BEGIN
    ALTER TABLE [dbo].[Interpreters] 
    ADD [Skills] NVARCHAR(MAX) NOT NULL DEFAULT '[]';
    PRINT 'Added Skills column to Interpreters table';
END

-- Add Status column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Interpreters' AND COLUMN_NAME = 'Status')
BEGIN
    ALTER TABLE [dbo].[Interpreters] 
    ADD [Status] NVARCHAR(MAX) NOT NULL DEFAULT 'Active';
    PRINT 'Added Status column to Interpreters table';
END

-- 2. Fix Clients table
PRINT 'Fixing Clients table...';

-- Add Preferences column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Clients' AND COLUMN_NAME = 'Preferences')
BEGIN
    ALTER TABLE [dbo].[Clients] 
    ADD [Preferences] NVARCHAR(MAX) NOT NULL DEFAULT '{}';
    PRINT 'Added Preferences column to Clients table';
END

-- Add Status column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Clients' AND COLUMN_NAME = 'Status')
BEGIN
    ALTER TABLE [dbo].[Clients] 
    ADD [Status] NVARCHAR(MAX) NOT NULL DEFAULT 'Active';
    PRINT 'Added Status column to Clients table';
END

-- 3. Fix Agencies table
PRINT 'Fixing Agencies table...';

-- Add Description column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Agencies' AND COLUMN_NAME = 'Description')
BEGIN
    ALTER TABLE [dbo].[Agencies] 
    ADD [Description] NVARCHAR(1000) NULL;
    PRINT 'Added Description column to Agencies table';
END

-- 4. Fix Appointments table
PRINT 'Fixing Appointments table...';

-- Add TimeZone column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'TimeZone')
BEGIN
    ALTER TABLE [dbo].[Appointments] 
    ADD [TimeZone] NVARCHAR(50) NOT NULL DEFAULT 'UTC';
    PRINT 'Added TimeZone column to Appointments table';
END

-- Add Status column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Status')
BEGIN
    ALTER TABLE [dbo].[Appointments] 
    ADD [Status] NVARCHAR(MAX) NOT NULL DEFAULT 'Scheduled';
    PRINT 'Added Status column to Appointments table';
END

-- Add Language column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Language')
BEGIN
    ALTER TABLE [dbo].[Appointments] 
    ADD [Language] NVARCHAR(50) NULL;
    PRINT 'Added Language column to Appointments table';
END

-- Add Rate column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Rate')
BEGIN
    ALTER TABLE [dbo].[Appointments] 
    ADD [Rate] DECIMAL(18,2) NULL;
    PRINT 'Added Rate column to Appointments table';
END

-- Add Notes column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Appointments' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE [dbo].[Appointments] 
    ADD [Notes] NVARCHAR(1000) NULL;
    PRINT 'Added Notes column to Appointments table';
END

-- 5. Fix Invoices table
PRINT 'Fixing Invoices table...';

-- Add Status column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Invoices' AND COLUMN_NAME = 'Status')
BEGIN
    ALTER TABLE [dbo].[Invoices] 
    ADD [Status] NVARCHAR(MAX) NOT NULL DEFAULT 'Pending';
    PRINT 'Added Status column to Invoices table';
END

-- Add Currency column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Invoices' AND COLUMN_NAME = 'Currency')
BEGIN
    ALTER TABLE [dbo].[Invoices] 
    ADD [Currency] NVARCHAR(3) NULL;
    PRINT 'Added Currency column to Invoices table';
END

-- Add Notes column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Invoices' AND COLUMN_NAME = 'Notes')
BEGIN
    ALTER TABLE [dbo].[Invoices] 
    ADD [Notes] NVARCHAR(1000) NULL;
    PRINT 'Added Notes column to Invoices table';
END

-- 6. Fix Notifications table
PRINT 'Fixing Notifications table...';

-- Add Type column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Notifications' AND COLUMN_NAME = 'Type')
BEGIN
    ALTER TABLE [dbo].[Notifications] 
    ADD [Type] NVARCHAR(MAX) NOT NULL DEFAULT 'Info';
    PRINT 'Added Type column to Notifications table';
END

-- Add Status column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Notifications' AND COLUMN_NAME = 'Status')
BEGIN
    ALTER TABLE [dbo].[Notifications] 
    ADD [Status] NVARCHAR(MAX) NOT NULL DEFAULT 'Unread';
    PRINT 'Added Status column to Notifications table';
END

-- Add Metadata column if missing
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Notifications' AND COLUMN_NAME = 'Metadata')
BEGIN
    ALTER TABLE [dbo].[Notifications] 
    ADD [Metadata] NVARCHAR(MAX) NOT NULL DEFAULT '{}';
    PRINT 'Added Metadata column to Notifications table';
END

-- 7. Create indexes for better performance
PRINT 'Creating performance indexes...';

-- Interpreters table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Interpreters]') AND name = N'IX_Interpreters_Availability')
BEGIN
    CREATE INDEX [IX_Interpreters_Availability] ON [dbo].[Interpreters] ([Availability]);
    PRINT 'Added index on Interpreters.Availability';
END

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Interpreters]') AND name = N'IX_Interpreters_Skills')
BEGIN
    CREATE INDEX [IX_Interpreters_Skills] ON [dbo].[Interpreters] ([Skills]);
    PRINT 'Added index on Interpreters.Skills';
END

-- Clients table indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'IX_Clients_Preferences')
BEGIN
    CREATE INDEX [IX_Clients_Preferences] ON [dbo].[Clients] ([Preferences]);
    PRINT 'Added index on Clients.Preferences';
END

PRINT 'Database column fixes complete!';
GO

-- Verify the changes
PRINT 'Verifying changes...';
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME IN ('Interpreters', 'Clients', 'Agencies', 'Appointments', 'Invoices', 'Notifications')
ORDER BY TABLE_NAME, COLUMN_NAME;
GO
