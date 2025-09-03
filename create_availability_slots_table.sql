-- Create AvailabilitySlots table and remove old serialized Availability column
-- This script converts from JSON serialization to proper relational structure

USE [ThatInterpretingAgency];
GO

PRINT 'Starting AvailabilitySlots table creation...';
GO

-- 1. Create the new AvailabilitySlots table
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'AvailabilitySlots')
BEGIN
    CREATE TABLE [dbo].[AvailabilitySlots] (
        [Id] uniqueidentifier NOT NULL,
        [InterpreterId] uniqueidentifier NOT NULL,
        [StartTime] datetime2 NOT NULL,
        [EndTime] datetime2 NOT NULL,
        [TimeZone] nvarchar(50) NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [Notes] nvarchar(1000) NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedAt] datetime2 NOT NULL,
        CONSTRAINT [PK_AvailabilitySlots] PRIMARY KEY ([Id])
    );
    
    PRINT 'Created AvailabilitySlots table';
    
    -- Create indexes for performance
    CREATE INDEX [IX_AvailabilitySlots_InterpreterId_StartTime_EndTime] 
    ON [dbo].[AvailabilitySlots] ([InterpreterId], [StartTime], [EndTime]);
    
    CREATE INDEX [IX_AvailabilitySlots_InterpreterId_Status] 
    ON [dbo].[AvailabilitySlots] ([InterpreterId], [Status]);
    
    CREATE INDEX [IX_AvailabilitySlots_StartTime_EndTime] 
    ON [dbo].[AvailabilitySlots] ([StartTime], [EndTime]);
    
    PRINT 'Created performance indexes for AvailabilitySlots table';
END
ELSE
BEGIN
    PRINT 'AvailabilitySlots table already exists';
END

-- 2. Add foreign key constraint to Interpreters table
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = N'FK_AvailabilitySlots_Interpreters_InterpreterId')
BEGIN
    ALTER TABLE [dbo].[AvailabilitySlots] 
    ADD CONSTRAINT [FK_AvailabilitySlots_Interpreters_InterpreterId] 
    FOREIGN KEY ([InterpreterId]) REFERENCES [dbo].[Interpreters] ([Id]) 
    ON DELETE CASCADE;
    
    PRINT 'Added foreign key constraint FK_AvailabilitySlots_Interpreters_InterpreterId';
END
ELSE
BEGIN
    PRINT 'Foreign key constraint already exists';
END

-- 3. Remove the old Availability column from Interpreters table if it exists
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
           WHERE TABLE_NAME = 'Interpreters' AND COLUMN_NAME = 'Availability')
BEGIN
    -- Drop any indexes on the Availability column first
    IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Interpreters]') AND name = N'IX_Interpreters_Availability')
    BEGIN
        DROP INDEX [IX_Interpreters_Availability] ON [dbo].[Interpreters];
        PRINT 'Dropped index IX_Interpreters_Availability';
    END
    
    -- Remove the Availability column
    ALTER TABLE [dbo].[Interpreters] DROP COLUMN [Availability];
    PRINT 'Removed old Availability column from Interpreters table';
END
ELSE
BEGIN
    PRINT 'Availability column does not exist in Interpreters table';
END

-- 4. Verify the new structure
PRINT 'Verifying new table structure...';

-- Check AvailabilitySlots table
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AvailabilitySlots'
ORDER BY COLUMN_NAME;

-- Check Interpreters table (should not have Availability column)
SELECT 
    TABLE_NAME,
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Interpreters' AND COLUMN_NAME = 'Availability';

-- Check foreign key constraints
SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,
    COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumnName
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) = 'AvailabilitySlots';

PRINT 'AvailabilitySlots table setup complete!';
GO
