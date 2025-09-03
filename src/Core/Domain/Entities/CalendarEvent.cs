using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class CalendarEvent : Entity
{
    public string UserId { get; private set; } = string.Empty;
    public Guid? ConnectionId { get; private set; }
    public string? ExternalEventId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Location { get; private set; }
    public DateTime StartTimeUtc { get; private set; }
    public DateTime EndTimeUtc { get; private set; }
    public string TimeZone { get; private set; } = string.Empty;
    public string EventType { get; private set; } = string.Empty; // 'Appointment', 'Availability', 'Blocked', 'Custom'
    public string Status { get; private set; } = string.Empty; // 'Active', 'Cancelled', 'Completed'
    public bool IsAllDay { get; private set; }
    public string? RecurrenceRule { get; private set; } // RRULE format
    public string? RecurrenceException { get; private set; } // JSON array of exception dates
    public string? Color { get; private set; } // Hex color code
    public int Priority { get; private set; }
    public int? ReminderMinutes { get; private set; }

    // Navigation properties
    public virtual UserCalendarConnection? Connection { get; private set; }
    public virtual ICollection<CalendarEventAttendee> Attendees { get; private set; } = new List<CalendarEventAttendee>();

    private CalendarEvent() { }

    public static CalendarEvent Create(string userId, string title, DateTime startTimeUtc, DateTime endTimeUtc, string timeZone, string eventType, Guid? connectionId = null, string? externalEventId = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (startTimeUtc >= endTimeUtc)
            throw new ArgumentException("Start time must be before end time", nameof(startTimeUtc));

        if (string.IsNullOrWhiteSpace(timeZone))
            throw new ArgumentException("Time zone cannot be empty", nameof(timeZone));

        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type cannot be empty", nameof(eventType));

        return new CalendarEvent
        {
            UserId = userId.Trim(),
            Title = title.Trim(),
            StartTimeUtc = startTimeUtc,
            EndTimeUtc = endTimeUtc,
            TimeZone = timeZone.Trim(),
            EventType = eventType.Trim(),
            Status = "Active",
            ConnectionId = connectionId,
            ExternalEventId = externalEventId?.Trim(),
            Priority = 0,
            IsAllDay = false
        };
    }

    public void UpdateDetails(string title, string? description, string? location)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        Title = title.Trim();
        Description = description?.Trim();
        Location = location?.Trim();
        UpdateTimestamp();
    }

    public void UpdateTimes(DateTime startTimeUtc, DateTime endTimeUtc)
    {
        if (startTimeUtc >= endTimeUtc)
            throw new ArgumentException("Start time must be before end time", nameof(startTimeUtc));

        StartTimeUtc = startTimeUtc;
        EndTimeUtc = endTimeUtc;
        UpdateTimestamp();
    }

    public void UpdateStatus(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
            throw new ArgumentException("Status cannot be empty", nameof(status));

        Status = status.Trim();
        UpdateTimestamp();
    }

    public void SetAllDay(bool isAllDay)
    {
        IsAllDay = isAllDay;
        UpdateTimestamp();
    }

    public void UpdateRecurrence(string? recurrenceRule, string? recurrenceException)
    {
        RecurrenceRule = recurrenceRule?.Trim();
        RecurrenceException = recurrenceException?.Trim();
        UpdateTimestamp();
    }

    public void SetColor(string? color)
    {
        Color = color?.Trim();
        UpdateTimestamp();
    }

    public void SetPriority(int priority)
    {
        Priority = priority;
        UpdateTimestamp();
    }

    public void SetReminder(int? reminderMinutes)
    {
        ReminderMinutes = reminderMinutes;
        UpdateTimestamp();
    }

    public void UpdateExternalId(string? externalEventId)
    {
        ExternalEventId = externalEventId?.Trim();
        UpdateTimestamp();
    }

    // Business logic methods
    public bool IsAppointment => EventType.Equals("Appointment", StringComparison.OrdinalIgnoreCase);
    public bool IsAvailability => EventType.Equals("Availability", StringComparison.OrdinalIgnoreCase);
    public bool IsBlocked => EventType.Equals("Blocked", StringComparison.OrdinalIgnoreCase);
    public bool IsCustom => EventType.Equals("Custom", StringComparison.OrdinalIgnoreCase);

    public bool IsActive => Status.Equals("Active", StringComparison.OrdinalIgnoreCase);
    public bool IsCancelled => Status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase);
    public bool IsCompleted => Status.Equals("Completed", StringComparison.OrdinalIgnoreCase);

    public TimeSpan Duration => EndTimeUtc - StartTimeUtc;

    public bool OverlapsWith(CalendarEvent other)
    {
        return StartTimeUtc < other.EndTimeUtc && EndTimeUtc > other.StartTimeUtc;
    }

    public bool IsInTimeRange(DateTime start, DateTime end)
    {
        return StartTimeUtc <= start && EndTimeUtc >= end;
    }

    public bool IsRecurring => !string.IsNullOrWhiteSpace(RecurrenceRule);


}
