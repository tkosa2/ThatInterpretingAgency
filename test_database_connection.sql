-- Test script to verify database connection and list available databases
-- Run this in SQL Server Management Studio

-- List all databases on the server
SELECT name, database_id, create_date, state_desc
FROM sys.databases
ORDER BY name;
GO

-- Check if we can connect to the specific database
IF DB_ID('ThatInterpretingAgency') IS NOT NULL
BEGIN
    PRINT 'Database ThatInterpretingAgency EXISTS and is accessible';
    
    -- Try to use the database
    USE [ThatInterpretingAgency];
    GO
    
    PRINT 'Successfully connected to database: ' + DB_NAME();
    PRINT 'Current user: ' + USER_NAME();
    
    -- List all tables in the database
    SELECT 
        TABLE_SCHEMA,
        TABLE_NAME,
        TABLE_TYPE
    FROM INFORMATION_SCHEMA.TABLES 
    ORDER BY TABLE_SCHEMA, TABLE_NAME;
    
    -- Check if UserProfiles table exists
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UserProfiles]') AND type in (N'U'))
    BEGIN
        PRINT 'UserProfiles table EXISTS in the database';
    END
    ELSE
    BEGIN
        PRINT 'UserProfiles table does NOT exist in the database';
    END
END
ELSE
BEGIN
    PRINT 'Database ThatInterpretingAgency does NOT exist or is not accessible';
    
    -- Check if there are similar database names
    SELECT name 
    FROM sys.databases 
    WHERE name LIKE '%ThatInterpreting%' OR name LIKE '%Interpreting%' OR name LIKE '%Agency%';
END
GO
