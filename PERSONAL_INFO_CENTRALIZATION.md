# Personal Information Centralization

This document outlines the changes made to centralize personal information fields (first name, last name, address, contact info) across the ThatInterpretingAgency codebase.

## Overview

Previously, personal information was scattered across multiple entities with inconsistent field names and structures. We've now centralized this information into two main value objects:

1. **`PersonalInfo`** - For individuals (interpreters, staff, etc.)
2. **`ContactInfo`** - For organizations/businesses (agencies, clients, etc.)

## New Value Objects

### PersonalInfo
Located in `src/Core/Domain/Common/PersonalInfo.cs`

**Fields:**
- `FirstName` (required)
- `LastName` (required)
- `MiddleName` (optional)
- `Email` (optional)
- `PhoneNumber` (optional)
- `Address` (optional)
- `City` (optional)
- `State` (optional)
- `ZipCode` (optional)
- `Country` (optional)

**Computed Properties:**
- `FullName` - Returns "FirstName LastName"
- `FullNameWithMiddle` - Returns "FirstName MiddleName LastName"

**Methods:**
- `Create()` - Static factory method
- `UpdateContactInfo()` - Update contact details
- `UpdateName()` - Update name fields

### ContactInfo
Located in `src/Core/Domain/Common/ContactInfo.cs`

**Fields:**
- `Name` (required) - Organization/business name
- `ContactPerson` (optional)
- `Email` (optional)
- `PhoneNumber` (optional)
- `Address` (optional)
- `City` (optional)
- `State` (optional)
- `ZipCode` (optional)
- `Country` (optional)
- `Website` (optional)

**Computed Properties:**
- `FullAddress` - Returns formatted address string

**Methods:**
- `Create()` - Static factory method
- `UpdateContactInfo()` - Update contact details
- `UpdateName()` - Update organization name

## Updated Entities

### AgencyAggregate
- Replaced individual fields (`Address`, `Phone`, `Email`) with `ContactInfo` value object
- Updated `Create()` method to accept `ContactInfo`
- Added `UpdateContactDetails()` method for granular updates
- Updated `AddInterpreter()` and `AddClient()` methods

### Client
- Replaced individual fields (`ContactPerson`, `PhoneNumber`, `Email`) with `ContactInfo` value object
- Updated `Create()` method to accept optional `ContactInfo`
- Added `UpdateContactInfo()` method
- Modified `UpdateOrganizationInfo()` method

### Interpreter
- Replaced `FullName` field with `PersonalInfo` value object
- Updated `Create()` method to accept `PersonalInfo`
- Added `UpdatePersonalInfo()` method

## Benefits of Centralization

1. **Consistency** - All personal information follows the same structure
2. **Maintainability** - Changes to personal info structure only need to be made in one place
3. **Validation** - Centralized validation logic for personal information
4. **Extensibility** - Easy to add new fields like `MiddleName`, `Country`, etc.
5. **Type Safety** - Strong typing prevents mixing up different types of contact information
6. **Reusability** - Value objects can be used across different entities

## Migration Notes

When updating existing code that uses these entities:

1. **AgencyAggregate**: Use `ContactInfo.Create()` to create contact information
2. **Client**: Use `ContactInfo.Create()` for organization contact details
3. **Interpreter**: Use `PersonalInfo.Create()` for individual interpreter information

## Example Usage

```csharp
// Creating an agency with contact info
var contactInfo = ContactInfo.Create(
    name: "ABC Interpreting Agency",
    contactPerson: "John Doe",
    email: "contact@abcagency.com",
    phoneNumber: "555-1234",
    address: "123 Main St",
    city: "Seattle",
    state: "WA",
    zipCode: "98101"
);

var agency = AgencyAggregate.Create("ABC Interpreting Agency", contactInfo);

// Creating an interpreter with personal info
var personalInfo = PersonalInfo.Create(
    firstName: "Jane",
    lastName: "Smith",
    email: "jane.smith@email.com",
    phoneNumber: "555-5678"
);

var interpreter = Interpreter.Create(agencyId, userId, personalInfo, skills);
```

## Next Steps

1. Update database migrations to reflect new structure
2. Update DTOs and view models to use new value objects
3. Update API controllers to handle new parameter structures
4. Update Blazor frontend components to work with new data structures
5. Add validation attributes and error handling for new value objects
