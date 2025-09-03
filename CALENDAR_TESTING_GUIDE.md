# Calendar Integration Testing Guide

## Overview
This guide explains how to test the basic calendar functionality that has been implemented for the THAT Interpreting Agency system.

## What's Been Implemented

### 1. **Database Infrastructure** ✅
- Calendar tables created and populated with default providers
- Foreign key constraints and indexes configured
- Cleanup trigger for user deletion

### 2. **Domain Entities** ✅
- `CalendarProvider` - Third-party calendar services
- `UserCalendarConnection` - User's connected calendars
- `CalendarEvent` - All calendar events (appointments, availability, etc.)
- `CalendarEventAttendee` - Event participants
- `CalendarSyncLog` - Sync operation tracking
- `CalendarTemplate` - Recurring availability patterns
- `CalendarTemplateRule` - Template time slot definitions

### 3. **Service Layer** ✅
- `ICalendarIntegrationService` interface
- `CalendarIntegrationService` implementation
- Basic CRUD operations for calendar events
- Availability management and conflict detection
- Placeholder implementations for external calendar integration

### 4. **API Endpoints** ✅
- `CalendarTestController` for testing basic functionality
- Endpoints for creating, reading, and managing calendar events
- Conflict detection and availability checking

## Testing the Basic Functionality

### **Prerequisites**
1. Ensure the database script has been run successfully
2. Start the API project (`src/Api`)
3. The API should be running on `https://localhost:7058`

### **Test Endpoints Available**

#### **1. Get Calendar Providers**
```http
GET https://localhost:7058/api/calendartest/providers
```
**Expected Result**: Returns the 4 default calendar providers (Outlook, Gmail, iCal, Custom)

#### **2. Create Test Availability Slot**
```http
POST https://localhost:7058/api/calendartest/test-availability
Content-Type: application/json

{
  "userId": "test-user-id",
  "startTime": "2024-01-15T09:00:00Z",
  "endTime": "2024-01-15T17:00:00Z",
  "timeZone": "America/New_York"
}
```
**Expected Result**: Creates an availability slot and returns the created event

#### **3. Create Test Appointment**
```http
POST https://localhost:7058/api/calendartest/test-appointment
Content-Type: application/json

{
  "userId": "test-user-id",
  "title": "Test Interpreting Session",
  "startTime": "2024-01-15T10:00:00Z",
  "endTime": "2024-01-15T12:00:00Z",
  "timeZone": "America/New_York"
}
```
**Expected Result**: Creates an appointment if the time slot is available

#### **4. Check for Conflicts**
```http
GET https://localhost:7058/api/calendartest/conflicts/test-user-id?startTime=2024-01-15T10:00:00Z&endTime=2024-01-15T12:00:00Z
```
**Expected Result**: Returns conflict information for the specified time slot

#### **5. Get User Events**
```http
GET https://localhost:7058/api/calendartest/events/test-user-id?startTime=2024-01-15T00:00:00Z&endTime=2024-01-16T00:00:00Z
```
**Expected Result**: Returns all events for the user in the specified time range

#### **6. Get User Availability**
```http
GET https://localhost:7058/api/calendartest/availability/test-user-id?startTime=2024-01-15T00:00:00Z&endTime=2024-01-16T00:00:00Z
```
**Expected Result**: Returns available time slots for the user

## Testing Scenarios

### **Scenario 1: Basic Availability Management**
1. Create an availability slot for a user
2. Verify the slot was created
3. Check that the slot appears in availability queries

### **Scenario 2: Appointment Creation**
1. Create an appointment in an available time slot
2. Verify the appointment was created
3. Check that the appointment appears in event queries

### **Scenario 3: Conflict Detection**
1. Try to create an appointment that overlaps with existing availability
2. Verify that the system detects the conflict
3. Check that the appointment creation fails with appropriate error

### **Scenario 4: Event Retrieval**
1. Create multiple events for a user
2. Query events by time range
3. Verify that all events are returned correctly

## Using the HTTP Test File

1. **Open the test file**: `test_calendar_functionality.http`
2. **Use VS Code REST Client extension** or import into Postman
3. **Execute tests in sequence** to verify functionality
4. **Check responses** against expected results

## Expected Test Results

### **Successful Operations**
- Availability slots created and stored
- Appointments created when time slots are available
- Events retrieved correctly by user and time range
- Conflict detection working properly

### **Error Handling**
- Validation errors for invalid data
- Conflict errors when trying to double-book
- Proper HTTP status codes and error messages

## Next Steps After Testing

Once basic functionality is verified:

1. **Implement external calendar integration** (Outlook, Gmail)
2. **Add OAuth 2.0 authentication**
3. **Create calendar UI components** in BlazorFrontend
4. **Implement advanced features** like recurring events and templates
5. **Add real-time sync** with external calendars

## Troubleshooting

### **Common Issues**
1. **Database connection errors**: Check connection string and ensure database is accessible
2. **Entity Framework errors**: Ensure all entities are properly configured in DbContext
3. **API not starting**: Check for compilation errors and missing dependencies

### **Debug Tips**
1. Check API logs for detailed error information
2. Use database queries to verify data is being stored correctly
3. Test individual endpoints to isolate issues

## Support

If you encounter issues during testing:
1. Check the API logs for error details
2. Verify database connectivity and schema
3. Ensure all required services are registered in DI container
4. Check that all entity configurations are correct in DbContext
