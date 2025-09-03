-- Add missing Availability column to Interpreters table
-- This column stores availability data as JSON to match the Entity Framework model

USE [ThatInterpretingAgency];
GO

PRINT 'Adding Availability column to Interpreters table...';

-- Check if column already exists
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
               WHERE TABLE_NAME = 'Interpreters' AND COLUMN_NAME = 'Availability')
BEGIN
    -- Add the Availability column
    ALTER TABLE [dbo].[Interpreters] 
    ADD [Availability] NVARCHAR(MAX) NOT NULL DEFAULT '[]';
    
    PRINT 'Added Availability column to Interpreters table';
    
    -- Add an index for better performance
    CREATE INDEX [IX_Interpreters_Availability] ON [dbo].[Interpreters] ([Availability]);
    PRINT 'Added index on Availability column';
END
ELSE
BEGIN
    PRINT 'Availability column already exists in Interpreters table';
END

-- Verify the column was added
SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Interpreters' AND COLUMN_NAME = 'Availability';

PRINT 'Availability column setup complete!';
GO
