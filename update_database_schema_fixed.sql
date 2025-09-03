-- Fixed SQL Script to update ThatInterpretingAgency database to relational schema
-- This script properly handles dropping dependent objects before changing column types
-- Run this script directly in SQL Server Management Studio or any SQL client

USE [ThatInterpretingAgency];
GO

PRINT 'Starting database schema update...';
GO

-- Step 1: Drop existing foreign key constraints and indexes that depend on UserId columns
PRINT 'Step 1: Dropping dependent foreign keys and indexes...';

-- Drop foreign key constraints first
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_AgencyStaff_AspNetUsers_UserId')
BEGIN
    ALTER TABLE [dbo].[AgencyStaff] DROP CONSTRAINT [FK_AgencyStaff_AspNetUsers_UserId];
    PRINT 'Dropped FK_AgencyStaff_AspNetUsers_UserId';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_Clients_AspNetUsers_UserId')
BEGIN
    ALTER TABLE [dbo].[Clients] DROP CONSTRAINT [FK_Clients_AspNetUsers_UserId];
    PRINT 'Dropped FK_Clients_AspNetUsers_UserId';
END

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_Interpreters_AspNetUsers_UserId')
BEGIN
    ALTER TABLE [dbo].[Interpreters] DROP CONSTRAINT [FK_Interpreters_AspNetUsers_UserId];
    PRINT 'Dropped FK_Interpreters_AspNetUsers_UserId';
END

-- Drop indexes that depend on UserId columns
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AgencyStaff]') AND name = N'IX_AgencyStaff_AgencyId_UserId_Role')
BEGIN
    DROP INDEX [IX_AgencyStaff_AgencyId_UserId_Role] ON [dbo].[AgencyStaff];
    PRINT 'Dropped IX_AgencyStaff_AgencyId_UserId_Role';
END

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AgencyStaff]') AND name = N'IX_AgencyStaff_UserId')
BEGIN
    DROP INDEX [IX_AgencyStaff_UserId] ON [dbo].[AgencyStaff];
    PRINT 'Dropped IX_AgencyStaff_UserId';
END

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'IX_Clients_AgencyId_UserId')
BEGIN
    DROP INDEX [IX_Clients_AgencyId_UserId] ON [dbo].[Clients];
    PRINT 'Dropped IX_Clients_AgencyId_UserId';
END

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'IX_Clients_UserId')
BEGIN
    DROP INDEX [IX_Clients_UserId] ON [dbo].[Clients];
    PRINT 'Dropped IX_Clients_UserId';
END

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Interpreters]') AND name = N'IX_Interpreters_AgencyId_UserId')
BEGIN
    DROP INDEX [IX_Interpreters_AgencyId_UserId] ON [dbo].[Interpreters];
    PRINT 'Dropped IX_Interpreters_AgencyId_UserId';
END

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Interpreters]') AND name = N'IX_Interpreters_UserId')
BEGIN
    DROP INDEX [IX_Interpreters_UserId] ON [dbo].[Interpreters];
    PRINT 'Dropped IX_Interpreters_UserId';
END

-- Drop indexes that depend on InterpreterId and ClientId columns in Appointments
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = N'IX_Appointments_AgencyId_InterpreterId_StartTime')
BEGIN
    DROP INDEX [IX_Appointments_AgencyId_InterpreterId_StartTime] ON [dbo].[Appointments];
    PRINT 'Dropped IX_Appointments_AgencyId_InterpreterId_StartTime';
END

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Appointments]') AND name = N'IX_Appointments_AgencyId_ClientId_StartTime')
BEGIN
    DROP INDEX [IX_Appointments_AgencyId_ClientId_StartTime] ON [dbo].[Appointments];
    PRINT 'Dropped IX_Appointments_AgencyId_ClientId_StartTime';
END

-- Drop indexes that depend on ClientId in Invoices
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Invoices]') AND name = N'IX_Invoices_AgencyId_ClientId')
BEGIN
    DROP INDEX [IX_Invoices_AgencyId_ClientId] ON [dbo].[Invoices];
    PRINT 'Dropped IX_Invoices_AgencyId_ClientId';
END

GO

-- Step 2: Change column types from uniqueidentifier to nvarchar(450)
PRINT 'Step 2: Changing column types...';

ALTER TABLE [dbo].[AgencyStaff] ALTER COLUMN [UserId] [nvarchar](450) NOT NULL;
PRINT 'Changed AgencyStaff.UserId to nvarchar(450)';

ALTER TABLE [dbo].[Clients] ALTER COLUMN [UserId] [nvarchar](450) NOT NULL;
PRINT 'Changed Clients.UserId to nvarchar(450)';

ALTER TABLE [dbo].[Interpreters] ALTER COLUMN [UserId] [nvarchar](450) NOT NULL;
PRINT 'Changed Interpreters.UserId to nvarchar(450)';

ALTER TABLE [dbo].[Appointments] ALTER COLUMN [InterpreterId] [nvarchar](450) NOT NULL;
PRINT 'Changed Appointments.InterpreterId to nvarchar(450)';

ALTER TABLE [dbo].[Appointments] ALTER COLUMN [ClientId] [nvarchar](450) NOT NULL;
PRINT 'Changed Appointments.ClientId to nvarchar(450)';

ALTER TABLE [dbo].[Invoices] ALTER COLUMN [ClientId] [nvarchar](450) NOT NULL;
PRINT 'Changed Invoices.ClientId to nvarchar(450)';

GO

-- Step 3: Remove old columns and add new ones
PRINT 'Step 3: Updating table structure...';

-- Add Description column to Agencies table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = N'Description')
BEGIN
    ALTER TABLE [dbo].[Agencies] ADD [Description] [nvarchar](1000) NULL;
    PRINT 'Added Description column to Agencies table';
END

-- Remove old columns from Agencies table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = N'ContactInfo')
BEGIN
    ALTER TABLE [dbo].[Agencies] DROP COLUMN [ContactInfo];
    PRINT 'Removed ContactInfo column from Agencies table';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = N'Address')
BEGIN
    ALTER TABLE [dbo].[Agencies] DROP COLUMN [Address];
    PRINT 'Removed Address column from Agencies table';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = N'Phone')
BEGIN
    ALTER TABLE [dbo].[Agencies] DROP COLUMN [Phone];
    PRINT 'Removed Phone column from Agencies table';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = N'Email')
BEGIN
    ALTER TABLE [dbo].[Agencies] DROP COLUMN [Email];
    PRINT 'Removed Email column from Agencies table';
END

-- Remove old columns from Interpreters table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Interpreters]') AND name = N'PersonalInfo')
BEGIN
    ALTER TABLE [dbo].[Interpreters] DROP COLUMN [PersonalInfo];
    PRINT 'Removed PersonalInfo column from Interpreters table';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Interpreters]') AND name = N'FullName')
BEGIN
    ALTER TABLE [dbo].[Interpreters] DROP COLUMN [FullName];
    PRINT 'Removed FullName column from Interpreters table';
END

-- Remove old columns from Clients table
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'ContactInfo')
BEGIN
    ALTER TABLE [dbo].[Clients] DROP COLUMN [ContactInfo];
    PRINT 'Removed ContactInfo column from Clients table';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'ContactPerson')
BEGIN
    ALTER TABLE [dbo].[Clients] DROP COLUMN [ContactPerson];
    PRINT 'Removed ContactPerson column from Clients table';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'Email')
BEGIN
    ALTER TABLE [dbo].[Clients] DROP COLUMN [Email];
    PRINT 'Removed Email column from Clients table';
END

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'PhoneNumber')
BEGIN
    ALTER TABLE [dbo].[Clients] DROP COLUMN [PhoneNumber];
    PRINT 'Removed PhoneNumber column from Clients table';
END

GO

-- Step 4: Recreate indexes
PRINT 'Step 4: Recreating indexes...';

-- Recreate indexes for UserId columns
CREATE NONCLUSTERED INDEX [IX_Interpreters_UserId] ON [dbo].[Interpreters]([UserId] ASC);
PRINT 'Created IX_Interpreters_UserId';

CREATE NONCLUSTERED INDEX [IX_Clients_UserId] ON [dbo].[Clients]([UserId] ASC);
PRINT 'Created IX_Clients_UserId';

CREATE NONCLUSTERED INDEX [IX_AgencyStaff_UserId] ON [dbo].[AgencyStaff]([UserId] ASC);
PRINT 'Created IX_AgencyStaff_UserId';

-- Recreate composite indexes
CREATE UNIQUE NONCLUSTERED INDEX [IX_Interpreters_AgencyId_UserId] ON [dbo].[Interpreters]([AgencyId], [UserId]);
PRINT 'Created IX_Interpreters_AgencyId_UserId';

CREATE UNIQUE NONCLUSTERED INDEX [IX_Clients_AgencyId_UserId] ON [dbo].[Clients]([AgencyId], [UserId]);
PRINT 'Created IX_Clients_AgencyId_UserId';

CREATE UNIQUE NONCLUSTERED INDEX [IX_AgencyStaff_AgencyId_UserId_Role] ON [dbo].[AgencyStaff]([AgencyId], [UserId], [Role]);
PRINT 'Created IX_AgencyStaff_AgencyId_UserId_Role';

-- Recreate indexes for Appointments
CREATE NONCLUSTERED INDEX [IX_Appointments_AgencyId_InterpreterId_StartTime] ON [dbo].[Appointments]([AgencyId], [InterpreterId], [StartTime]);
PRINT 'Created IX_Appointments_AgencyId_InterpreterId_StartTime';

CREATE NONCLUSTERED INDEX [IX_Appointments_AgencyId_ClientId_StartTime] ON [dbo].[Appointments]([AgencyId], [ClientId], [StartTime]);
PRINT 'Created IX_Appointments_AgencyId_ClientId_StartTime';

-- Recreate indexes for Invoices
CREATE NONCLUSTERED INDEX [IX_Invoices_AgencyId_ClientId] ON [dbo].[Invoices]([AgencyId], [ClientId]);
PRINT 'Created IX_Invoices_AgencyId_ClientId';

GO

-- Step 5: Add foreign key constraints to UserProfiles table
PRINT 'Step 5: Adding foreign key constraints...';

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_AgencyStaff_UserProfiles_UserId')
BEGIN
    ALTER TABLE [dbo].[AgencyStaff] ADD CONSTRAINT [FK_AgencyStaff_UserProfiles_UserId] 
    FOREIGN KEY([UserId]) REFERENCES [dbo].[UserProfiles] ([UserId]) ON DELETE NO ACTION;
    PRINT 'Added FK_AgencyStaff_UserProfiles_UserId';
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_Clients_UserProfiles_UserId')
BEGIN
    ALTER TABLE [dbo].[Clients] ADD CONSTRAINT [FK_Clients_UserProfiles_UserId] 
    FOREIGN KEY([UserId]) REFERENCES [dbo].[UserProfiles] ([UserId]) ON DELETE NO ACTION;
    PRINT 'Added FK_Clients_UserProfiles_UserId';
END

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_Interpreters_UserProfiles_UserId')
BEGIN
    ALTER TABLE [dbo].[Interpreters] ADD CONSTRAINT [FK_Interpreters_UserProfiles_UserId] 
    FOREIGN KEY([UserId]) REFERENCES [dbo].[UserProfiles] ([UserId]) ON DELETE NO ACTION;
    PRINT 'Added FK_Interpreters_UserProfiles_UserId';
END

GO

-- Step 6: Create migration history table if it doesn't exist
PRINT 'Step 6: Updating migration history...';

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory](
        [MigrationId] [nvarchar](150) NOT NULL,
        [ProductVersion] [nvarchar](32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
    );
    PRINT 'Created __EFMigrationsHistory table';
END

-- Insert migration records to mark these changes as applied
IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20250825000520_UpdateToRelationalSchema')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250825000520_UpdateToRelationalSchema', N'9.0.0');
    PRINT 'Added migration record to __EFMigrationsHistory';
END

GO

-- Step 7: Verify the changes
PRINT 'Step 7: Verifying changes...';

-- List all tables
SELECT 
    TABLE_SCHEMA,
    TABLE_NAME,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES 
ORDER BY TABLE_SCHEMA, TABLE_NAME;

-- Check UserProfiles table structure
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'UserProfiles'
ORDER BY ORDINAL_POSITION;

PRINT 'Database schema updated successfully!';
PRINT 'UserProfiles table created and existing tables updated.';
PRINT 'All foreign key relationships and indexes have been established.';
GO
