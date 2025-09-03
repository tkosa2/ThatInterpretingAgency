-- Simple script to create UserProfiles table
-- Run this in SQL Server Management Studio against your ThatInterpretingAgency database

USE [ThatInterpretingAgency];
GO

-- Check if we can connect to the database
IF DB_ID('ThatInterpretingAgency') IS NULL
BEGIN
    PRINT 'ERROR: Database ThatInterpretingAgency does not exist!';
    RETURN;
END
GO

PRINT 'Connected to database: ' + DB_NAME();
GO

-- Check current user permissions
PRINT 'Current user: ' + USER_NAME();
GO

-- Create UserProfiles table
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND type in (N'U'))
BEGIN
    PRINT 'Creating UserProfiles table...';
    
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
    
    PRINT 'UserProfiles table created successfully!';
END
ELSE
BEGIN
    PRINT 'UserProfiles table already exists.';
END
GO

-- Create unique constraint on UserId
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND name = N'AK_UserProfiles_UserId')
BEGIN
    PRINT 'Creating unique index on UserId...';
    CREATE UNIQUE NONCLUSTERED INDEX [AK_UserProfiles_UserId] ON [dbo].[UserProfiles]([UserId] ASC);
    PRINT 'Unique index created successfully!';
END
ELSE
BEGIN
    PRINT 'Unique index on UserId already exists.';
END
GO

-- Verify the table was created
SELECT 
    TABLE_NAME,
    TABLE_SCHEMA,
    TABLE_TYPE
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'UserProfiles';
GO

-- Show table structure
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'UserProfiles'
ORDER BY ORDINAL_POSITION;
GO

PRINT 'Script completed successfully!';
GO
