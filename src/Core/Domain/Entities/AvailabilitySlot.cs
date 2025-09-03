using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class AvailabilitySlot : Entity
{
    public Guid InterpreterId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string TimeZone { get; private set; } = string.Empty;
    public AvailabilityStatus Status { get; private set; }
    public string? Notes { get; private set; }

    // Navigation property
    public virtual Interpreter Interpreter { get; private set; } = null!;

    private AvailabilitySlot() { }

    public static AvailabilitySlot Create(Guid interpreterId, DateTime startTime, DateTime endTime, string timeZone, string? notes = null)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time", nameof(startTime));

        if (string.IsNullOrWhiteSpace(timeZone))
            throw new ArgumentException("Time zone cannot be empty", nameof(timeZone));

        return new AvailabilitySlot
        {
            InterpreterId = interpreterId,
            StartTime = startTime,
            EndTime = endTime,
            TimeZone = timeZone.Trim(),
            Status = AvailabilityStatus.Available,
            Notes = notes?.Trim()
        };
    }

    public void UpdateTimes(DateTime startTime, DateTime endTime)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time", nameof(startTime));

        StartTime = startTime;
        EndTime = endTime;
        UpdateTimestamp();
    }

    public void UpdateStatus(AvailabilityStatus newStatus)
    {
        Status = newStatus;
        UpdateTimestamp();
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes?.Trim();
        UpdateTimestamp();
    }

    public bool OverlapsWith(AvailabilitySlot other)
    {
        return StartTime < other.EndTime && EndTime > other.StartTime;
    }

    public bool IsInTimeRange(DateTime start, DateTime end)
    {
        return StartTime <= start && EndTime >= end;
    }

    public TimeSpan Duration => EndTime - StartTime;


}

public enum AvailabilityStatus
{
    Available,
    Booked,
    Unavailable,
    Pending
}
