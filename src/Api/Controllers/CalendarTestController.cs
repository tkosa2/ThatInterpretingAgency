using Microsoft.AspNetCore.Mvc;
using ThatInterpretingAgency.Core.Application.Services;
using ThatInterpretingAgency.Core.Domain.Entities;

namespace ThatInterpretingAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CalendarTestController : ControllerBase
{
    private readonly ICalendarIntegrationService _calendarService;

    public CalendarTestController(ICalendarIntegrationService calendarService)
    {
        _calendarService = calendarService;
    }

    [HttpGet("providers")]
    public async Task<IActionResult> GetProviders()
    {
        try
        {
            var providers = await _calendarService.GetActiveProvidersAsync();
            return Ok(providers);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("test-user")]
    public async Task<IActionResult> GetTestUser()
    {
        try
        {
            // This endpoint returns a test user ID that can be used for testing
            // In a real application, you would get this from the authenticated user context
            var testUserId = "test-user-12345"; // This is just for testing
            
            return Ok(new { 
                userId = testUserId,
                message = "Use this userId for testing calendar endpoints",
                note = "In production, this would come from the authenticated user's context"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("create-test-user")]
    public async Task<IActionResult> CreateTestUser()
    {
        try
        {
            // This is a temporary solution for testing - creates a test user in the database
            // In production, you would use proper user management
            
            // For now, let's use a simple approach - we'll create a test user with a known ID
            var testUserId = Guid.NewGuid().ToString();
            
            // You would need to implement this method in your service layer
            // For now, we'll return the test user ID that should be created
            return Ok(new { 
                userId = testUserId,
                message = "Test user ID generated. You need to manually create this user in the database.",
                sqlCommand = $"INSERT INTO AspNetUsers (Id, UserName, Email, EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount) VALUES ('{testUserId}', 'testuser', 'test@example.com', 1, 1, 0, 0, 0)"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("events")]
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
    {
        try
        {
            // Create a test calendar event
            var calendarEvent = CalendarEvent.Create(
                request.UserId,
                request.Title,
                request.StartTime,
                request.EndTime,
                request.TimeZone,
                request.EventType
            );

            var createdEvent = await _calendarService.CreateEventAsync(calendarEvent);
            return Ok(createdEvent);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("events/{userId}")]
    public async Task<IActionResult> GetUserEvents(string userId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        try
        {
            var events = await _calendarService.GetUserEventsAsync(userId, startTime, endTime);
            return Ok(events);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("availability/{userId}")]
    public async Task<IActionResult> GetAvailability(string userId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        try
        {
            var availableSlots = await _calendarService.GetAvailableSlotsAsync(userId, startTime, endTime);
            return Ok(availableSlots);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPost("test-availability")]
    public async Task<IActionResult> CreateTestAvailability([FromBody] CreateAvailabilityRequest request)
    {
        try
        {
            // Create a test availability slot
            var availabilityEvent = CalendarEvent.Create(
                request.UserId,
                "Available for Interpreting",
                request.StartTime,
                request.EndTime,
                request.TimeZone,
                "Availability"
            );

            var createdEvent = await _calendarService.CreateEventAsync(availabilityEvent);
            return Ok(createdEvent);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("test-appointment")]
    public async Task<IActionResult> CreateTestAppointment([FromBody] CreateAppointmentRequest request)
    {
        try
        {
            // Check if the time slot is available
            var isAvailable = await _calendarService.IsTimeSlotAvailableAsync(request.UserId, request.StartTime, request.EndTime);
            
            if (!isAvailable)
            {
                var conflicts = await _calendarService.GetConflictsAsync(request.UserId, request.StartTime, request.EndTime);
                return BadRequest(new { error = "Time slot not available", conflicts });
            }

            // Create the appointment
            var appointmentEvent = CalendarEvent.Create(
                request.UserId,
                request.Title,
                request.StartTime,
                request.EndTime,
                request.TimeZone,
                "Appointment"
            );

            var createdEvent = await _calendarService.CreateEventAsync(appointmentEvent);
            return Ok(createdEvent);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("conflicts/{userId}")]
    public async Task<IActionResult> CheckConflicts(string userId, [FromQuery] DateTime startTime, [FromQuery] DateTime endTime)
    {
        try
        {
            var hasConflicts = await _calendarService.HasConflictsAsync(userId, startTime, endTime);
            var conflicts = await _calendarService.GetConflictsAsync(userId, startTime, endTime);
            
            return Ok(new { hasConflicts, conflicts });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}

public class CreateEventRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string TimeZone { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
}

public class CreateAvailabilityRequest
{
    public string UserId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string TimeZone { get; set; } = string.Empty;
}

public class CreateAppointmentRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string TimeZone { get; set; } = string.Empty;
}
