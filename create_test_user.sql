-- Create a test user for calendar functionality testing
-- This script inserts a test user into the AspNetUsers table

INSERT INTO AspNetUsers (
    Id, 
    UserName, 
    Email, 
    EmailConfirmed, 
    PhoneNumberConfirmed, 
    TwoFactorEnabled, 
    LockoutEnabled, 
    AccessFailedCount, 
    NormalizedUserName, 
    NormalizedEmail, 
    SecurityStamp, 
    ConcurrencyStamp, 
    PhoneNumber, 
    PasswordHash
)
VALUES (
    'test-user-12345', 
    'testuser', 
    'test@example.com', 
    1, 
    1, 
    0, 
    0, 
    0,
    'TESTUSER',
    'TEST@EXAMPLE.COM',
    'TEST-STAMP-123',
    'TEST-CONCURRENCY-123',
    NULL,
    'AQAAAAIAAYagAAAAELbXpF8XqQ=='
);

-- Verify the user was created
SELECT Id, UserName, Email, EmailConfirmed FROM AspNetUsers WHERE Id = 'test-user-12345';

