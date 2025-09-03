# Availability Refactoring Guide

## Overview
This guide explains the changes made to convert the Availability system from JSON serialization to a proper relational table structure.

## What Changed

### 1. **AvailabilitySlot Entity**
- **Before**: `AvailabilitySlot` was a `ValueObject` stored as serialized JSON in the `Interpreters` table
- **After**: `AvailabilitySlot` is now a proper `Entity` with its own table and relationships

### 2. **Database Structure**
- **Before**: `Interpreters.Availability` column (NVARCHAR(MAX)) storing JSON
- **After**: `AvailabilitySlots` table with proper foreign key relationships

### 3. **Interpreter Entity**
- **Before**: `public List<AvailabilitySlot> Availability { get; private set; }`
- **After**: `public virtual ICollection<AvailabilitySlot> AvailabilitySlots { get; private set; }`

## Files Modified

### Core Domain
- `src/Core/Domain/Entities/AvailabilitySlot.cs` - Converted from ValueObject to Entity
- `src/Core/Domain/Entities/Interpreter.cs` - Updated to use collection relationship

### Infrastructure
- `src/Infrastructure/Persistence/ThatInterpretingAgencyDbContext.cs` - Added AvailabilitySlot configuration
- `src/Infrastructure/Persistence/Repositories/IAvailabilitySlotRepository.cs` - New repository interface
- `src/Infrastructure/Persistence/Repositories/AvailabilitySlotRepository.cs` - New repository implementation
- `src/Infrastructure/Persistence/Repositories/InterpreterRepository.cs` - Updated to use AvailabilitySlots

### API & BFF
- `src/Api/Program.cs` - Registered new repository
- `src/Bff/Program.cs` - Registered new repository

## Database Migration Steps

### Step 1: Run the SQL Script
Execute `create_availability_slots_table.sql` in your SQL Server database:

```sql
-- This script will:
-- 1. Create the new AvailabilitySlots table
-- 2. Add proper indexes for performance
-- 3. Remove the old Availability column from Interpreters
-- 4. Set up foreign key constraints
```

### Step 2: Verify the Changes
The script will show you:
- New table structure
- Confirmation that old column was removed
- Foreign key constraints

## Benefits of the New Structure

### 1. **Better Performance**
- Proper indexes on StartTime, EndTime, InterpreterId
- No JSON parsing overhead
- Efficient queries for availability checks

### 2. **Data Integrity**
- Foreign key constraints ensure referential integrity
- Proper data types for dates and times
- Cascade delete when interpreter is removed

### 3. **Easier Querying**
- Can use SQL date/time functions
- Better support for complex availability queries
- Easier to implement business logic

### 4. **Scalability**
- No JSON size limitations
- Better for reporting and analytics
- Easier to implement caching

## Usage Examples

### Adding Availability
```csharp
// Old way (JSON serialization)
interpreter.AddAvailabilitySlot(startTime, endTime, timeZone);

// New way (relational)
var slot = AvailabilitySlot.Create(interpreter.Id, startTime, endTime, timeZone);
await _availabilitySlotRepository.AddAsync(slot);
```

### Querying Availability
```csharp
// Old way
var hasAvailability = interpreter.Availability.Any(slot => 
    slot.Status == AvailabilityStatus.Available &&
    slot.StartTime <= startTime &&
    slot.EndTime >= endTime);

// New way
var availableSlots = await _availabilitySlotRepository.GetAvailableSlotsAsync(
    interpreterId, startTime, endTime);
var hasAvailability = availableSlots.Any();
```

### Checking for Overlaps
```csharp
// New way with repository
var hasOverlap = await _availabilitySlotRepository.HasOverlappingSlotsAsync(
    interpreterId, startTime, endTime, excludeSlotId);
```

## Testing the Changes

### 1. **Build the Project**
```bash
dotnet build
```

### 2. **Run Database Script**
Execute the SQL script in SQL Server Management Studio

### 3. **Test the API**
Start the application and test availability endpoints

### 4. **Verify Data**
Check that the new table structure is working correctly

## Rollback Plan

If you need to rollback:
1. Restore the old `AvailabilitySlot` ValueObject
2. Add back the `Availability` column to `Interpreters` table
3. Revert the DbContext configuration
4. Remove the new repository registrations

## Next Steps

1. **Update Controllers**: Modify any controllers that directly access availability data
2. **Update Services**: Update business logic services to use the new repository
3. **Add Validation**: Implement business rules for availability management
4. **Performance Testing**: Test the new structure with larger datasets

## Questions?

If you encounter any issues during the migration:
1. Check the SQL script output for any errors
2. Verify the database schema matches the Entity Framework model
3. Ensure all repository dependencies are properly registered
4. Check that the foreign key constraints are created correctly
