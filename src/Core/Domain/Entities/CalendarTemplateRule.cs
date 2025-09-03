using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class CalendarTemplateRule : Entity
{
    public Guid TemplateId { get; private set; }
    public int? DayOfWeek { get; private set; } // 0=Sunday, 1=Monday, etc.
    public TimeSpan StartTime { get; private set; }
    public TimeSpan EndTime { get; private set; }
    public string TimeZone { get; private set; } = string.Empty;
    public string EventType { get; private set; } = string.Empty; // Default: 'Availability'
    public int Priority { get; private set; }

    // Navigation properties
    public virtual CalendarTemplate Template { get; private set; } = null!;

    private CalendarTemplateRule() { }

    public static CalendarTemplateRule Create(Guid templateId, TimeSpan startTime, TimeSpan endTime, string timeZone, int? dayOfWeek = null, string eventType = "Availability", int priority = 0)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time", nameof(startTime));

        if (string.IsNullOrWhiteSpace(timeZone))
            throw new ArgumentException("Time zone cannot be empty", nameof(timeZone));

        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type cannot be empty", nameof(eventType));

        if (dayOfWeek.HasValue && (dayOfWeek.Value < 0 || dayOfWeek.Value > 6))
            throw new ArgumentException("Day of week must be between 0 and 6", nameof(dayOfWeek));

        return new CalendarTemplateRule
        {
            TemplateId = templateId,
            DayOfWeek = dayOfWeek,
            StartTime = startTime,
            EndTime = endTime,
            TimeZone = timeZone.Trim(),
            EventType = eventType.Trim(),
            Priority = priority
        };
    }

    public void UpdateTimes(TimeSpan startTime, TimeSpan endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time", nameof(startTime));

        StartTime = startTime;
        EndTime = endTime;
        UpdateTimestamp();
    }

    public void UpdateDayOfWeek(int? dayOfWeek)
    {
        if (dayOfWeek.HasValue && (dayOfWeek.Value < 0 || dayOfWeek.Value > 6))
            throw new ArgumentException("Day of week must be between 0 and 6", nameof(dayOfWeek));

        DayOfWeek = dayOfWeek;
        UpdateTimestamp();
    }

    public void UpdateEventType(string eventType)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException("Event type cannot be empty", nameof(eventType));

        EventType = eventType.Trim();
        UpdateTimestamp();
    }

    public void UpdatePriority(int priority)
    {
        Priority = priority;
        UpdateTimestamp();
    }

    public void UpdateTimeZone(string timeZone)
    {
        if (string.IsNullOrWhiteSpace(timeZone))
            throw new ArgumentException("Time zone cannot be empty", nameof(timeZone));

        TimeZone = timeZone.Trim();
        UpdateTimestamp();
    }

    // Business logic methods
    public bool IsWeeklyRule => DayOfWeek.HasValue;
    public bool IsDailyRule => !DayOfWeek.HasValue;

    public TimeSpan Duration => EndTime - StartTime;

    public bool IsAvailabilityRule => EventType.Equals("Availability", StringComparison.OrdinalIgnoreCase);
    public bool IsAppointmentRule => EventType.Equals("Appointment", StringComparison.OrdinalIgnoreCase);
    public bool IsBlockedRule => EventType.Equals("Blocked", StringComparison.OrdinalIgnoreCase);

    public string DayOfWeekName => DayOfWeek switch
    {
        0 => "Sunday",
        1 => "Monday",
        2 => "Tuesday",
        3 => "Wednesday",
        4 => "Thursday",
        5 => "Friday",
        6 => "Saturday",
        _ => "Daily"
    };


}
