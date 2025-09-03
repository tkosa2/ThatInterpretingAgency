using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class Interpreter : Entity
{
    public Guid AgencyId { get; private set; }
    public string UserId { get; private set; } = string.Empty; // Changed to string to match AspNetUsers.Id
    public List<string> Skills { get; private set; } = new();
    public InterpreterStatus Status { get; private set; }

    // Navigation properties
    public virtual UserProfile UserProfile { get; private set; } = null!;
    public virtual ICollection<AvailabilitySlot> AvailabilitySlots { get; private set; } = new List<AvailabilitySlot>();

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

    public void AddAvailabilitySlot(DateTime startTime, DateTime endTime, string timeZone, string? notes = null)
    {
        if (startTime >= endTime)
            throw new ArgumentException("Start time must be before end time");

        if (startTime < DateTime.UtcNow)
            throw new ArgumentException("Start time cannot be in the past");

        var slot = AvailabilitySlot.Create(Id, startTime, endTime, timeZone, notes);
        AvailabilitySlots.Add(slot);
        UpdateTimestamp();
    }

    public void RemoveAvailabilitySlot(Guid slotId)
    {
        var slot = AvailabilitySlots.FirstOrDefault(s => s.Id == slotId);
        if (slot != null)
        {
            AvailabilitySlots.Remove(slot);
            UpdateTimestamp();
        }
    }

    public bool IsAvailable(DateTime startTime, DateTime endTime)
    {
        if (Status != InterpreterStatus.Active)
            return false;

        var requestedSlot = new { Start = startTime, End = endTime };
        
        return AvailabilitySlots.Any(slot => 
            slot.Status == AvailabilityStatus.Available &&
            slot.StartTime <= requestedSlot.Start &&
            slot.EndTime >= requestedSlot.End);
    }

    public IEnumerable<AvailabilitySlot> GetAvailableSlots(DateTime startTime, DateTime endTime)
    {
        return AvailabilitySlots.Where(slot => 
            slot.Status == AvailabilityStatus.Available &&
            slot.StartTime <= startTime &&
            slot.EndTime >= endTime);
    }
}

public enum InterpreterStatus
{
    Active,
    Inactive,
    Suspended,
    OnLeave
}
