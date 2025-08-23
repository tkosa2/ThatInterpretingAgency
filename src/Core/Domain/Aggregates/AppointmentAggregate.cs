using ThatInterpretingAgency.Core.Domain.Common;
using ThatInterpretingAgency.Core.Domain.ValueObjects;

namespace ThatInterpretingAgency.Core.Domain.Aggregates;

public class AppointmentAggregate : AggregateRoot
{
    public Guid AgencyId { get; private set; }
    public Guid InterpreterId { get; private set; }
    public Guid ClientId { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string TimeZone { get; private set; } = string.Empty;
    public AppointmentStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public string? Location { get; private set; }
    public string? Language { get; private set; }
    public decimal? Rate { get; private set; }
    public string? CancellationReason { get; private set; }

    private AppointmentAggregate() { }

    public static AppointmentAggregate Create(
        Guid agencyId, 
        Guid interpreterId, 
        Guid clientId, 
        DateTime startTime, 
        DateTime endTime, 
        string timeZone,
        string? location = null,
        string? language = null,
        decimal? rate = null,
        string? notes = null)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time", nameof(startTime));

        if (startTime < DateTime.UtcNow)
            throw new ArgumentException("Start time cannot be in the past", nameof(startTime));

        if (string.IsNullOrWhiteSpace(timeZone))
            throw new ArgumentException("Time zone cannot be empty", nameof(timeZone));

        var appointment = new AppointmentAggregate
        {
            AgencyId = agencyId,
            InterpreterId = interpreterId,
            ClientId = clientId,
            StartTime = startTime,
            EndTime = endTime,
            TimeZone = timeZone.Trim(),
            Status = AppointmentStatus.Scheduled,
            Location = location?.Trim(),
            Language = language?.Trim(),
            Rate = rate,
            Notes = notes?.Trim()
        };

        return appointment;
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled appointments can be confirmed");

        Status = AppointmentStatus.Confirmed;
        UpdateTimestamp();
    }

    public void Start()
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed appointments can be started");

        if (DateTime.UtcNow < StartTime)
            throw new InvalidOperationException("Cannot start appointment before scheduled time");

        Status = AppointmentStatus.InProgress;
        UpdateTimestamp();
    }

    public void Complete()
    {
        if (Status != AppointmentStatus.InProgress)
            throw new InvalidOperationException("Only in-progress appointments can be started");

        Status = AppointmentStatus.Completed;
        UpdateTimestamp();
    }

    public void Cancel(string reason)
    {
        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Completed appointments cannot be cancelled");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason is required", nameof(reason));

        Status = AppointmentStatus.Cancelled;
        CancellationReason = reason.Trim();
        UpdateTimestamp();
    }

    public void Reschedule(DateTime newStartTime, DateTime newEndTime)
    {
        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Completed appointments cannot be rescheduled");

        if (newStartTime >= newEndTime)
            throw new ArgumentException("New start time must be before new end time");

        if (newStartTime < DateTime.UtcNow)
            throw new ArgumentException("New start time cannot be in the past");

        StartTime = newStartTime;
        EndTime = newEndTime;
        Status = AppointmentStatus.Rescheduled;
        UpdateTimestamp();
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes?.Trim();
        UpdateTimestamp();
    }

    public void UpdateLocation(string location)
    {
        Location = location?.Trim();
        UpdateTimestamp();
    }

    public void UpdateRate(decimal rate)
    {
        if (rate < 0)
            throw new ArgumentException("Rate cannot be negative", nameof(rate));

        Rate = rate;
        UpdateTimestamp();
    }

    public TimeSpan Duration => EndTime - StartTime;

    public bool IsOverlapping(DateTime start, DateTime end)
    {
        return StartTime < end && EndTime > start;
    }
}

public enum AppointmentStatus
{
    Scheduled,
    Confirmed,
    InProgress,
    Completed,
    Cancelled,
    Rescheduled,
    NoShow
}
