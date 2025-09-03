-- SQL Script to update ThatInterpretingAgency database to relational schema
-- Run this script directly in SQL Server Management Studio or any SQL client
-- This script will add the UserProfiles table and update existing tables

USE [ThatInterpretingAgency];
GO

-- Create UserProfiles table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[UserProfiles](
        [Id] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
        [UserId] [nvarchar](450) NOT NULL,
        [FirstName] [nvarchar](100) NOT NULL,
        [LastName] [nvarchar](100) NOT NULL,
        [MiddleName] [nvarchar](100) NULL,
        [MailingAddress] [nvarchar](500) NULL,
        [PhysicalAddress] [nvarchar](500) NULL,
        [City] [nvarchar](100) NULL,
        [State] [nvarchar](100) NULL,
        [ZipCode] [nvarchar](20) NULL,
        [Country] [nvarchar](100) NULL,
        [CreatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        [UpdatedAt] [datetime2](7) NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_UserProfiles] PRIMARY KEY CLUSTERED ([Id] ASC)
    );
END
GO

-- Create unique constraint on UserId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = N'AK_UserProfiles_UserId')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [AK_UserProfiles_UserId] ON [dbo].[UserProfiles]([UserId] ASC);
END
GO

-- Add Description column to Agencies table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = N'Description')
BEGIN
    ALTER TABLE [dbo].[Agencies] ADD [Description] [nvarchar](1000) NULL;
END
GO

-- Remove ContactInfo column from Agencies table if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = N'ContactInfo')
BEGIN
    ALTER TABLE [dbo].[Agencies] DROP COLUMN [ContactInfo];
END
GO

-- Remove PersonalInfo column from Interpreters table if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Interpreters]') AND name = N'PersonalInfo')
BEGIN
    ALTER TABLE [dbo].[Interpreters] DROP COLUMN [PersonalInfo];
END
GO

-- Remove ContactInfo column from Clients table if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'ContactInfo')
BEGIN
    ALTER TABLE [dbo].[Clients] DROP COLUMN [ContactInfo];
END
GO

-- Remove FullName column from Interpreters table if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Interpreters]') AND name = N'FullName')
BEGIN
    ALTER TABLE [dbo].[Interpreters] DROP COLUMN [FullName];
END
GO

-- Remove ContactPerson column from Clients table if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'ContactPerson')
BEGIN
    ALTER TABLE [dbo].[Clients] DROP COLUMN [ContactPerson];
END
GO

-- Remove Email column from Clients table if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'Email')
BEGIN
    ALTER TABLE [dbo].[Clients] DROP COLUMN [Email];
END
GO

-- Remove PhoneNumber column from Clients table if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'PhoneNumber')
BEGIN
    ALTER TABLE [dbo].[Clients] DROP COLUMN [PhoneNumber];
END
GO

-- Remove Address column from Agencies table if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = N'Address')
BEGIN
    ALTER TABLE [dbo].[Agencies] DROP COLUMN [Address];
END
GO

-- Remove Phone column from Agencies table if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = N'Phone')
BEGIN
    ALTER TABLE [dbo].[Agencies] DROP COLUMN [Phone];
END
GO

-- Remove Email column from Agencies table if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = N'Email')
BEGIN
    ALTER TABLE [dbo].[Agencies] DROP COLUMN [Email];
END
GO

-- Change UserId columns from uniqueidentifier to nvarchar(450)
-- First, drop existing foreign key constraints if they exist
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_AgencyStaff_AspNetUsers_UserId')
BEGIN
    ALTER TABLE [dbo].[AgencyStaff] DROP CONSTRAINT [FK_AgencyStaff_AspNetUsers_UserId];
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_Clients_AspNetUsers_UserId')
BEGIN
    ALTER TABLE [dbo].[Clients] DROP CONSTRAINT [FK_Clients_AspNetUsers_UserId];
END
GO

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_Interpreters_AspNetUsers_UserId')
BEGIN
    ALTER TABLE [dbo].[Interpreters] DROP CONSTRAINT [FK_Interpreters_AspNetUsers_UserId];
END
GO

-- Change UserId column types
ALTER TABLE [dbo].[AgencyStaff] ALTER COLUMN [UserId] [nvarchar](450) NOT NULL;
GO

ALTER TABLE [dbo].[Clients] ALTER COLUMN [UserId] [nvarchar](450) NOT NULL;
GO

ALTER TABLE [dbo].[Interpreters] ALTER COLUMN [UserId] [nvarchar](450) NOT NULL;
GO

-- Change ClientId and InterpreterId columns in Appointments table
ALTER TABLE [dbo].[Appointments] ALTER COLUMN [InterpreterId] [nvarchar](450) NOT NULL;
GO

ALTER TABLE [dbo].[Appointments] ALTER COLUMN [ClientId] [nvarchar](450) NOT NULL;
GO

-- Change ClientId column in Invoices table
ALTER TABLE [dbo].[Invoices] ALTER COLUMN [ClientId] [nvarchar](450) NOT NULL;
GO

-- Create indexes for UserId columns
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Interpreters]') AND name = N'IX_Interpreters_UserId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Interpreters_UserId] ON [dbo].[Interpreters]([UserId] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Clients]') AND name = N'IX_Clients_UserId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Clients_UserId] ON [dbo].[Clients]([UserId] ASC);
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AgencyStaff]') AND name = N'IX_AgencyStaff_UserId')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_AgencyStaff_UserId] ON [dbo].[AgencyStaff]([UserId] ASC);
END
GO

-- Add foreign key constraints to UserProfiles table
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_AgencyStaff_UserProfiles_UserId')
BEGIN
    ALTER TABLE [dbo].[AgencyStaff] ADD CONSTRAINT [FK_AgencyStaff_UserProfiles_UserId] 
    FOREIGN KEY([UserId]) REFERENCES [dbo].[UserProfiles] ([UserId]) ON DELETE NO ACTION;
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_Clients_UserProfiles_UserId')
BEGIN
    ALTER TABLE [dbo].[Clients] ADD CONSTRAINT [FK_Clients_UserProfiles_UserId] 
    FOREIGN KEY([UserId]) REFERENCES [dbo].[UserProfiles] ([UserId]) ON DELETE NO ACTION;
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_Interpreters_UserProfiles_UserId')
BEGIN
    ALTER TABLE [dbo].[Interpreters] ADD CONSTRAINT [FK_Interpreters_UserProfiles_UserId] 
    FOREIGN KEY([UserId]) REFERENCES [dbo].[UserProfiles] ([UserId]) ON DELETE NO ACTION;
END
GO

-- Create migration history table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[__EFMigrationsHistory]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory](
        [MigrationId] [nvarchar](150) NOT NULL,
        [ProductVersion] [nvarchar](32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId] ASC)
    );
END
GO

-- Insert migration records to mark these changes as applied
IF NOT EXISTS (SELECT * FROM [dbo].[__EFMigrationsHistory] WHERE [MigrationId] = N'20250825000520_UpdateToRelationalSchema')
BEGIN
    INSERT INTO [dbo].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250825000520_UpdateToRelationalSchema', N'9.0.0');
END
GO

PRINT 'Database schema updated successfully!';
PRINT 'UserProfiles table created and existing tables updated.';
GO
