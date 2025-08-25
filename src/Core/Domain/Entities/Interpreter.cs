using ThatInterpretingAgency.Core.Domain.Common;
using ThatInterpretingAgency.Core.Domain.ValueObjects;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class Interpreter : Entity
{
    public Guid AgencyId { get; private set; }
    public string UserId { get; private set; } = string.Empty; // Changed to string to match AspNetUsers.Id
    public List<string> Skills { get; private set; } = new();
    public InterpreterStatus Status { get; private set; }
    public List<AvailabilitySlot> Availability { get; private set; } = new();

    // Navigation properties
    public virtual UserProfile UserProfile { get; private set; } = null!;

    private Interpreter() { }

    public static Interpreter Create(Guid agencyId, string userId, List<string> skills)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (skills == null || !skills.Any())
            throw new ArgumentException("At least one skill must be specified", nameof(skills));

        return new Interpreter
        {
            AgencyId = agencyId,
            UserId = userId.Trim(),
            Skills = skills.Select(s => s.Trim()).ToList(),
            Status = InterpreterStatus.Active
        };
    }

    public void UpdateSkills(List<string> newSkills)
    {
        if (newSkills == null || !newSkills.Any())
            throw new ArgumentException("At least one skill must be specified", nameof(newSkills));

        Skills = newSkills.Select(s => s.Trim()).ToList();
        UpdateTimestamp();
    }

    public void UpdateStatus(InterpreterStatus newStatus)
    {
        Status = newStatus;
        UpdateTimestamp();
    }

    public void AddAvailabilitySlot(DateTime startTime, DateTime endTime, string timeZone)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time");

        if (startTime < DateTime.UtcNow)
            throw new ArgumentException("Start time cannot be in the past");

        var slot = AvailabilitySlot.Create(startTime, endTime, timeZone);
        Availability.Add(slot);
        UpdateTimestamp();
    }

    public void RemoveAvailabilitySlot(DateTime startTime, DateTime endTime, string timeZone)
    {
        var slot = Availability.FirstOrDefault(s => 
            s.StartTime == startTime && 
            s.EndTime == endTime && 
            s.TimeZone == timeZone);
        if (slot != null)
        {
            Availability.Remove(slot);
            UpdateTimestamp();
        }
    }

    public bool IsAvailable(DateTime startTime, DateTime endTime)
    {
        if (Status != InterpreterStatus.Active)
            return false;

        var requestedSlot = new { Start = startTime, End = endTime };
        
        return Availability.Any(slot => 
            slot.Status == AvailabilityStatus.Available &&
            slot.StartTime <= requestedSlot.Start &&
            slot.EndTime >= requestedSlot.End);
    }
}

public enum InterpreterStatus
{
    Active,
    Inactive,
    Suspended,
    OnLeave
}
