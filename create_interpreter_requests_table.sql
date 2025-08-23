-- Create InterpreterRequests table
CREATE TABLE [InterpreterRequests] (
    [Id] uniqueidentifier NOT NULL,
    [AgencyId] uniqueidentifier NOT NULL,
    [RequestorId] uniqueidentifier NOT NULL,
    [AppointmentType] nvarchar(50) NOT NULL,
    [VirtualMeetingLink] nvarchar(500) NULL,
    [Location] nvarchar(500) NULL,
    [Mode] nvarchar(50) NULL,
    [Description] nvarchar(max) NULL,
    [RequestedDate] datetime2 NOT NULL,
    [StartTime] datetime2 NOT NULL,
    [EndTime] datetime2 NOT NULL,
    [Language] nvarchar(100) NOT NULL,
    [SpecialInstructions] nvarchar(max) NULL,
    [Status] nvarchar(50) NOT NULL,
    [Division] nvarchar(100) NULL,
    [Program] nvarchar(100) NULL,
    [LniContact] nvarchar(100) NULL,
    [DayOfEventContact] nvarchar(100) NULL,
    [DayOfEventContactPhone] nvarchar(50) NULL,
    [CostCode] nvarchar(50) NULL,
    [InvoiceApprover] nvarchar(100) NULL,
    [SupportingMaterials] bit NOT NULL,
    [AppointmentId] uniqueidentifier NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_InterpreterRequests] PRIMARY KEY ([Id])
);

-- Create indexes
CREATE INDEX [IX_InterpreterRequests_AgencyId] ON [InterpreterRequests] ([AgencyId]);
CREATE INDEX [IX_InterpreterRequests_RequestorId] ON [InterpreterRequests] ([RequestorId]);
CREATE INDEX [IX_InterpreterRequests_AgencyId_Status] ON [InterpreterRequests] ([AgencyId], [Status]);
CREATE INDEX [IX_InterpreterRequests_AgencyId_Language] ON [InterpreterRequests] ([AgencyId], [Language]);
CREATE INDEX [IX_InterpreterRequests_AgencyId_RequestedDate] ON [InterpreterRequests] ([AgencyId], [RequestedDate]);

-- Add check constraints
ALTER TABLE [InterpreterRequests] ADD CONSTRAINT [CHK_InterpreterRequests_Mode] 
    CHECK ([Mode] IN ('Consecutive', 'Simultaneous') OR [Mode] IS NULL);

ALTER TABLE [InterpreterRequests] ADD CONSTRAINT [CHK_InterpreterRequests_AppointmentType] 
    CHECK ([AppointmentType] IN ('In-Person', 'Virtual'));

ALTER TABLE [InterpreterRequests] ADD CONSTRAINT [CHK_InterpreterRequests_Status] 
    CHECK ([Status] IN ('Pending', 'Approved', 'Rejected', 'Fulfilled', 'Cancelled'));

-- Add foreign key constraints (if the referenced tables exist)
-- Note: These might fail if the referenced tables don't have the expected structure
-- ALTER TABLE [InterpreterRequests] ADD CONSTRAINT [FK_InterpreterRequests_Agencies] 
--     FOREIGN KEY ([AgencyId]) REFERENCES [Agencies] ([Id]) ON DELETE CASCADE;

-- ALTER TABLE [InterpreterRequests] ADD CONSTRAINT [FK_InterpreterRequests_Clients] 
--     FOREIGN KEY ([RequestorId]) REFERENCES [Clients] ([Id]) ON DELETE CASCADE;
