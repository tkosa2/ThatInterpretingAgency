# User Stories for THAT Interpreting Agency - CHECKLIST

This document outlines user stories for the THAT Interpreting Agency application, a global platform for managing interpreting services. The application uses **ASP.NET Core 9** with **Blazor Server** for the frontend and backend, **Microsoft SQL Server (MSSQL)** for the database, and **QuickBooks Online** for billing. It follows **Domain-Driven Design (DDD)** with **vertical slice architecture** for modularity. Stories are grouped by bounded context (Agency Management, Request Management, Interpreter Management, Client Management, Scheduling, Billing, Notifications) and include frontend-specific requirements for Blazor Server components. Each story includes acceptance criteria to guide development and testing, prioritized using MoSCoW (Must-have, Should-have, Could-have, Won't-have). Development uses **Visual Studio Code** with free extensions (e.g., C#, SQL Server, Blazor Snippets).

## Personas
- **Agency Admin**: Manages agency operations, including staff, interpreters, clients, requests, and billing.
- **Interpreter**: Provides interpreting services, manages availability, and receives payments via QuickBooks.
- **Client**: Requests and books interpreters for appointments and pays invoices via QuickBooks.

## Agency Management

### US-1: Create Agency (Must-have)
**As an Agency Admin**, I want to create a new agency so that I can manage interpreters and clients under THAT Interpreting Agency.

**Acceptance Criteria**:
- [X] Backend: Create an agency with Name, ContactInfo, and unique Id in MSSQL Agencies table.
- [X] Backend: Validate that Name is unique and not empty.
- [] Backend: Restrict to authenticated admins via ASP.NET Core Identity.
- [X] Frontend: Build a Blazor Server component (e.g., `CreateAgency.razor`) with an EditForm for Name and ContactInfo.
- [X] Frontend: Use `@inject HttpClient Http` to call POST /api/agencies, handling agency-scoped authentication.
- [X] Frontend: Display validation errors (e.g., duplicate Name). SHOW EXACT ERROR MESSAGE "Agency is already existed.  Please use different agency name."
- [X] Frontend: Edit agency information.

### US-2: Assign Staff to Agency (Must-have)
**As an Agency Admin**, I want to assign users as staff (e.g., interpreters, admins) to an agency so that they can perform agency-specific tasks.

**Acceptance Criteria**:
- [] Backend: Add a user to AgencyStaff table with AgencyId, UserId, Role (e.g., Interpreter, Admin), HireDate, and Status.
- [] Backend: Validate user and agency exist; ensure no duplicate role assignment.
- [] Backend: Restrict to admins of the same agency.
- [] Frontend: Create a Blazor Server component (e.g., `AssignStaff.razor`) with a dropdown for users and roles.
- [] Frontend: Use `@inject HttpClient Http` to call POST /api/agency-staff, ensuring agency scoping.
- [] Frontend: Show success/error messages.

## Request Management

### US-15: Create Interpreter Request (Must-have)
**As a Client**, I want to create an interpreter request so that I can specify my interpreting needs for review by the agency.

**Acceptance Criteria**:
- [] Backend: Create a request with AgencyId, RequestorId, AppointmentType, VirtualMeetingLink, Location, Mode, Description, RequestedDate, StartTime, EndTime, Language, SpecialInstructions, Status, and optional fields (Division, Program, LniContact, DayOfEventContact, DayOfEventContactPhone, CostCode, InvoiceApprover, SupportingMaterials) in MSSQL InterpreterRequests table.
- [] Backend: Validate RequestorId is a client in the agency; ensure valid AppointmentType (In-Person, Virtual) and Mode (Consecutive, Simultaneous).
- [] Backend: Store times in UTC; set Status to 'Pending'.
- [] Frontend: Build a Blazor Server component (e.g., `CreateInterpreterRequest.razor`) with an EditForm for request details, including DatePicker for RequestedDate and Start/EndTime.
- [] Frontend: Use `@inject HttpClient Http` to call POST /api/requests, ensuring agency scoping.
- [] Frontend: Display validation errors (e.g., invalid date, missing Language).

### US-16: Manage Interpreter Requests (Must-have)
**As an Agency Admin**, I want to manage interpreter requests so that I can approve, reject, or fulfill them with appointments.

**Acceptance Criteria**:
- [X] Backend: API supports updating request Status (Pending, Approved, Rejected, Fulfilled) and linking to an AppointmentId if fulfilled.
- [X] Backend: Validate agency scoping; ensure only admins can manage requests.
- [X] Backend: If Status is 'Fulfilled', create an appointment in MSSQL Appointments table with matching details.
- [X] Frontend: Create a Blazor Server component (e.g., `ManageRequests.razor`) with a table of requests and buttons for Approve/Reject/Fulfill.
- [X] Frontend: Use `@inject HttpClient Http` to call PUT /api/requests/{id}/status and POST /api/scheduling/appointments for fulfillment.
- [X] Frontend: Display request details and status updates; handle errors.

### US-17: View and Cancel Interpreter Request (Must-have)
**As a Client**, I want to view and cancel my interpreter requests so that I can manage my pending requests.

**Acceptance Criteria**:
- [X] Backend: API returns all interpreter requests for the authenticated client (by RequestorId), filtered by AgencyId.
- [X] Backend: Allow updating request Status to 'Cancelled' if Status is 'Pending'; validate agency scoping.
- [X] Backend: Trigger notification to agency admin on cancellation.
- [X] Backend: Prevent cancellation if Status is 'Approved', 'Rejected', or 'Fulfilled'.
- [X] Frontend: Build a Blazor Server component (e.g., `ClientRequests.razor`) with a table of requests (e.g., Language, RequestedDate, Status) and a Cancel button for Pending requests.
- [X] Frontend: Use `@inject HttpClient Http` to call GET /api/requests/client and PUT /api/requests/{id}/cancel.
- [X] Frontend: Display time slots in client's time zone (using `DateTimeOffset`); show error if cancellation is not allowed.
- [X] Frontend: Show success message on cancellation and update UI in real-time via SignalR.

## Interpreter Management

### US-3: Create Interpreter Profile (Must-have)
**As an Agency Admin**, I want to create interpreter profiles for THAT Interpreting Agency so that I can assign certified interpreters to clients.

**Acceptance Criteria**:
- [X] Backend: Create interpreter with UserId, AgencyId, FullName, and Skills in MSSQL Interpreters table.
- [X] Backend: Validate UserId is in AgencyStaff (Role: Interpreter) and no duplicate UserId.
- [X] Frontend: Build a Blazor Server component (e.g., `CreateInterpreter.razor`) with an EditForm for FullName and Skills.
- [X] Frontend: Use `@inject HttpClient Http` to call POST /api/interpreters, passing AgencyId from User claims.
- [X] Frontend: Display validation errors (e.g., invalid AgencyId).

### US-4: Update Interpreter Availability (Must-have)
**As an Interpreter**, I want to update my availability across time zones so that I can be scheduled for appointments without conflicts.

**Acceptance Criteria**:
- [X] Backend: Add/update availability slots (StartTime, EndTime) in MSSQL, stored in UTC.
- [X] Backend: Validate no overlapping slots; restrict to interpreter's agency.
- [X] Frontend: Create a Blazor Server component (e.g., `AvailabilityCalendar.razor`) with a calendar UI (e.g., using Blazorise DatePicker).
- [X] Frontend: Convert local time to UTC for API calls; display in user's time zone (using `DateTimeOffset`).
- [X] Frontend: Use `@inject HttpClient Http` to call PUT /api/interpreters/availability, handling errors.

## Client Management

### US-5: Create Client Profile (Must-have)
**As an Agency Admin**, I want to create client profiles so that clients can book interpreters.

**Acceptance Criteria**:
- [X] Backend: Create client with UserId, AgencyId, OrganizationName, and Preferences in MSSQL Clients table.
- [X] Backend: Validate UserId and AgencyId; ensure no duplicate UserId.
- [X] Frontend: Build a Blazor Server component (e.g., `CreateClient.razor`) with an EditForm for OrganizationName and Preferences.
- [X] Frontend: Use `@inject HttpClient Http` to call POST /api/clients, ensuring agency scoping.
- [X] Frontend: Show validation errors.

### US-6: Update Client Preferences (Should-have)
**As a Client**, I want to update my preferences (e.g., language, location) so that I can find suitable interpreters.

**Acceptance Criteria**:
- [X] Backend: Update Preferences in MSSQL Clients table; validate agency scoping.
- [X] Frontend: Create a Blazor Server component (e.g., `ClientPreferences.razor`) with an EditForm for preferences.
- [X] Frontend: Use `@inject HttpClient Http` to call PUT /api/clients/preferences, displaying success/error.
- [X] Frontend: Support multi-language UI (e.g., using Blazor's localization).

## Scheduling

### US-7: Book Appointment (Must-have)
**As a Client**, I want to book an appointment from an approved interpreter request so that I can confirm interpreting services.

**Acceptance Criteria**:
- [X] Backend: API returns approved interpreter requests; create appointment with AgencyId, InterpreterRequestId, InterpreterId, ClientId, TimeSlot, and Status in MSSQL.
- [X] Backend: Validate agency scoping, no overlaps; update request Status to 'Fulfilled'; trigger notification.
- [X] Frontend: Build a Blazor Server component (e.g., `BookAppointment.razor`) with filters (language, location) and a DatePicker.
- [X] Frontend: Use `@inject HttpClient Http` to call GET /api/requests/approved and POST /api/scheduling/appointments.
- [X] Frontend: Display time slots in client's time zone; show error for unavailable slots.

### US-8: View Available Time Slots (Should-have)
**As a Client**, I want to view available time slots for an interpreter so that I can choose a convenient time for an appointment.

**Acceptance Criteria**:
- [X] Backend: API returns time slots filtered by AgencyId, interpreter availability, and existing appointments.
- [X] Backend: Store times in UTC.
- [X] Frontend: Create a Blazor Server component (e.g., `TimeSlotPicker.razor`) with a grid or calendar UI.
- [X] Frontend: Use `@inject HttpClient Http` to call GET /api/scheduling/timeslots, converting UTC to local time.
- [X] Frontend: Handle empty slot lists gracefully.

## Billing

### US-9: Trigger Invoice Creation in QuickBooks (Must-have)
**As an Agency Admin**, I want to trigger invoice creation in QuickBooks for completed appointments so that clients receive bills automatically.

**Acceptance Criteria**:
- [X] Backend: Send appointment details (ClientId, AppointmentId, duration, rate) to QuickBooks API; store QuickBooksInvoiceId in MSSQL Invoices table.
- [X] Backend: Validate agency scoping; handle API errors with retries.
- [X] Frontend: Create a Blazor Server component (e.g., `CreateInvoice.razor`) with a button to trigger invoice creation.
- [X] Frontend: Use `@inject HttpClient Http` to call POST /api/billing/invoices, displaying success/error.
- [X] Frontend: Restrict to admins via ASP.NET Core authentication.

### US-10: Receive Invoice from QuickBooks (Must-have)
**As a Client**, I want to receive invoices from QuickBooks so that I can pay securely.

**Acceptance Criteria**:
- [X] Backend: Log invoice creation in MSSQL Notifications table after QuickBooks API call.
- [X] Backend: No financial data stored locally, only QuickBooksInvoiceId.
- [X] Frontend: Display invoice notification in a Blazor Server component (e.g., `InvoiceNotification.razor`) with QuickBooks payment link.
- [X] Frontend: Use `@inject HttpClient Http` to poll GET /api/billing/invoices for status updates.

### US-11: Receive Payment via QuickBooks (Must-have)
**As an Interpreter**, I want QuickBooks to handle my payments so that I receive funds for completed appointments.

**Acceptance Criteria**:
- [X] Backend: Trigger QuickBooks payment to interpreter after client payment; log in Notifications table.
- [X] Backend: Validate interpreter is in AgencyStaff (Role: Interpreter).
- [X] Frontend: Create a Blazor Server component (e.g., `PaymentStatus.razor`) to show payment status from QuickBooks.
- [X] Frontend: Use `@inject HttpClient Http` to call GET /api/billing/payments, displaying status.

### US-12: View Invoice List (Should-have)
**As an Agency Admin**, I want to view a list of invoices with QuickBooks Invoice IDs so that I can track billing status.

**Acceptance Criteria**:
- [X] Backend: API returns invoices by AgencyId, showing QuickBooksInvoiceId and AppointmentId.
- [X] Backend: Query QuickBooks API for payment status.
- [X] Frontend: Build a Blazor Server component (e.g., `InvoiceList.razor`) with a table of invoices.
- [X] Frontend: Use `@inject HttpClient Http` to call GET /api/billing/invoices, ensuring agency scoping.

## Notifications

### US-13: Send Appointment Reminder (Must-have)
**As an Interpreter or Client**, I want to receive appointment reminders via email or SMS so that I don't miss bookings.

**Acceptance Criteria**:
- [X] Backend: Trigger notification (Type: Email/SMS) 24 hours before appointment; store in MSSQL Notifications table.
- [X] Backend: Use SendGrid/Twilio for delivery; validate agency scoping.
- [X] Frontend: Display notification status in a Blazor Server component (e.g., `NotificationLog.razor`).
- [X] Frontend: Use `@inject HttpClient Http` to call GET /api/notifications for status.

### US-14: Receive Invoice Notification (Should-have)
**As a Client**, I want to receive a notification when an invoice is created so that I can pay promptly.

**Acceptance Criteria**:
- [X] Backend: Log notification in MSSQL Notifications table after QuickBooks invoice creation.
- [X] Backend: Send email/SMS via SendGrid/Twilio with QuickBooks link.
- [X] Frontend: Update `InvoiceNotification.razor` to show invoice creation alert.
- [X] Frontend: Use `@inject HttpClient Http` to poll GET /api/notifications for updates.

## Prioritization
- **Must-have**: US-1, US-2, US-3, US-4, US-5, US-7, US-9, US-10, US-11, US-13, US-15, US-16, US-17
- **Should-have**: US-6, US-8, US-12, US-14
- **Could-have**: Additional filters (e.g., interpreter ratings).
- **Won't-have**: Manual invoice creation (handled by QuickBooks).

## Development Notes
- **Frontend**: Use Blazor Server with Razor components, Blazorise for UI (e.g., DatePicker, DataGrid), and ASP.NET Core authentication for agency scoping. Leverage SignalR for real-time updates (e.g., request status, cancellations).
- **Backend**: Implement vertical slices (e.g., `CreateInterpreterRequestCommand`, `CancelInterpreterRequestCommand`) with ASP.NET Core 9, MediatR, and EF Core.
- **MSSQL**: Apply schema from `update_schema.sql` (see README.md).
- **QuickBooks**: Integrate with QuickBooks Online API for US-9, US-10, US-11.
- **Testing**: Write xUnit tests for backend; bUnit for Blazor components.
- **Tools**: Use Visual Studio Code with extensions:
  - C# (ms-dotnettools.csharp)
  - SQL Server (ms-mssql.mssql)
  - Blazor Snippets (adrianwilczynski.blazor-snippet-pack)
  - REST Client (humao.rest-client)
  - Prettier (esbenp.prettier-vscode)
