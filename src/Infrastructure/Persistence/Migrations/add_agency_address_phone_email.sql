-- Add Address, Phone, and Email columns to Agencies table
-- This script adds the missing properties that are now part of the AgencyAggregate domain model

USE [ThatInterpretingAgency]
GO

-- Add Address column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = 'Address')
BEGIN
    ALTER TABLE [dbo].[Agencies] ADD [Address] [nvarchar](500) NULL
END
GO

-- Add Phone column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = 'Phone')
BEGIN
    ALTER TABLE [dbo].[Agencies] ADD [Phone] [nvarchar](20) NULL
END
GO

-- Add Email column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Agencies]') AND name = 'Email')
BEGIN
    ALTER TABLE [dbo].[Agencies] ADD [Email] [nvarchar](100) NULL
END
GO

-- Update existing agencies with default values if needed
UPDATE [dbo].[Agencies] 
SET [Address] = COALESCE([Address], ''),
    [Phone] = COALESCE([Phone], ''),
    [Email] = COALESCE([Email], '')
WHERE [Address] IS NULL OR [Phone] IS NULL OR [Email] IS NULL
GO
