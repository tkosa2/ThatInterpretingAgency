using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class CalendarSyncLog : Entity
{
    public Guid ConnectionId { get; private set; }
    public string SyncType { get; private set; } = string.Empty; // 'Full', 'Incremental', 'Manual'
    public string Status { get; private set; } = string.Empty; // 'Success', 'Failed', 'Partial'
    public DateTime StartTime { get; private set; }
    public DateTime? EndTime { get; private set; }
    public int EventsProcessed { get; private set; }
    public int EventsCreated { get; private set; }
    public int EventsUpdated { get; private set; }
    public int EventsDeleted { get; private set; }
    public string? ErrorMessage { get; private set; }

    // Navigation properties
    public virtual UserCalendarConnection Connection { get; private set; } = null!;

    private CalendarSyncLog() { }

    public static CalendarSyncLog Create(Guid connectionId, string syncType)
    {
        if (string.IsNullOrWhiteSpace(syncType))
            throw new ArgumentException("Sync type cannot be empty", nameof(syncType));

        return new CalendarSyncLog
        {
            ConnectionId = connectionId,
            SyncType = syncType.Trim(),
            Status = "Started",
            StartTime = DateTime.UtcNow,
            EventsProcessed = 0,
            EventsCreated = 0,
            EventsUpdated = 0,
            EventsDeleted = 0
        };
    }

    public void CompleteSuccess(int eventsProcessed, int eventsCreated, int eventsUpdated, int eventsDeleted)
    {
        Status = "Success";
        EndTime = DateTime.UtcNow;
        EventsProcessed = eventsProcessed;
        EventsCreated = eventsCreated;
        EventsUpdated = eventsUpdated;
        EventsDeleted = eventsDeleted;
        UpdateTimestamp();
    }

    public void CompletePartial(int eventsProcessed, int eventsCreated, int eventsUpdated, int eventsDeleted, string? errorMessage = null)
    {
        Status = "Partial";
        EndTime = DateTime.UtcNow;
        EventsProcessed = eventsProcessed;
        EventsCreated = eventsCreated;
        EventsUpdated = eventsUpdated;
        EventsDeleted = eventsDeleted;
        ErrorMessage = errorMessage;
        UpdateTimestamp();
    }

    public void CompleteFailure(string errorMessage)
    {
        Status = "Failed";
        EndTime = DateTime.UtcNow;
        ErrorMessage = errorMessage;
        UpdateTimestamp();
    }

    public void UpdateProgress(int eventsProcessed, int eventsCreated, int eventsUpdated, int eventsDeleted)
    {
        EventsProcessed = eventsProcessed;
        EventsCreated = eventsCreated;
        EventsUpdated = eventsUpdated;
        EventsDeleted = eventsDeleted;
        UpdateTimestamp();
    }

    // Business logic methods
    public bool IsStarted => Status.Equals("Started", StringComparison.OrdinalIgnoreCase);
    public bool IsSuccess => Status.Equals("Success", StringComparison.OrdinalIgnoreCase);
    public bool IsFailed => Status.Equals("Failed", StringComparison.OrdinalIgnoreCase);
    public bool IsPartial => Status.Equals("Partial", StringComparison.OrdinalIgnoreCase);
    public bool IsCompleted => EndTime.HasValue;

    public TimeSpan? Duration => EndTime?.Subtract(StartTime);

    public bool IsFullSync => SyncType.Equals("Full", StringComparison.OrdinalIgnoreCase);
    public bool IsIncrementalSync => SyncType.Equals("Incremental", StringComparison.OrdinalIgnoreCase);
    public bool IsManualSync => SyncType.Equals("Manual", StringComparison.OrdinalIgnoreCase);


}
