using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class CalendarEventAttendee : Entity
{
    public Guid EventId { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string? Name { get; private set; }
    public string ResponseStatus { get; private set; } = string.Empty; // 'Pending', 'Accepted', 'Declined', 'Tentative'
    public DateTime? ResponseTime { get; private set; }
    public bool IsOrganizer { get; private set; }

    // Navigation properties
    public virtual CalendarEvent Event { get; private set; } = null!;

    private CalendarEventAttendee() { }

    public static CalendarEventAttendee Create(Guid eventId, string email, string? name = null, bool isOrganizer = false)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        return new CalendarEventAttendee
        {
            EventId = eventId,
            Email = email.Trim(),
            Name = name?.Trim(),
            ResponseStatus = "Pending",
            IsOrganizer = isOrganizer
        };
    }

    public void UpdateResponse(string responseStatus, DateTime? responseTime = null)
    {
        if (string.IsNullOrWhiteSpace(responseStatus))
            throw new ArgumentException("Response status cannot be empty", nameof(responseStatus));

        ResponseStatus = responseStatus.Trim();
        ResponseTime = responseTime ?? DateTime.UtcNow;
        UpdateTimestamp();
    }

    public void SetAsOrganizer(bool isOrganizer)
    {
        IsOrganizer = isOrganizer;
        UpdateTimestamp();
    }

    public void UpdateName(string? name)
    {
        Name = name?.Trim();
        UpdateTimestamp();
    }

    // Business logic methods
    public bool IsPending => ResponseStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase);
    public bool IsAccepted => ResponseStatus.Equals("Accepted", StringComparison.OrdinalIgnoreCase);
    public bool IsDeclined => ResponseStatus.Equals("Declined", StringComparison.OrdinalIgnoreCase);
    public bool IsTentative => ResponseStatus.Equals("Tentative", StringComparison.OrdinalIgnoreCase);

    public bool HasResponded => !IsPending;


}
