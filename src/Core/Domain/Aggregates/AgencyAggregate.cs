using ThatInterpretingAgency.Core.Domain.Common;
using ThatInterpretingAgency.Core.Domain.Entities;

namespace ThatInterpretingAgency.Core.Domain.Aggregates;

public class AgencyAggregate : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public AgencyStatus Status { get; private set; }
    public List<AgencyStaff> Staff { get; private set; } = new();
    public List<Interpreter> Interpreters { get; private set; } = new();
    public List<Client> Clients { get; private set; } = new();

    private AgencyAggregate() { }

    public static AgencyAggregate Create(string name, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Agency name cannot be empty", nameof(name));

        var agency = new AgencyAggregate
        {
            Name = name.Trim(),
            Description = description?.Trim(),
            Status = AgencyStatus.Active
        };

        return agency;
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Agency name cannot be empty", nameof(name));

        Name = name.Trim();
        UpdateTimestamp();
    }

    public void UpdateDescription(string? description)
    {
        Description = description?.Trim();
        UpdateTimestamp();
    }

    public void Deactivate()
    {
        Status = AgencyStatus.Inactive;
        UpdateTimestamp();
    }

    public void Activate()
    {
        Status = AgencyStatus.Active;
        UpdateTimestamp();
    }

    public void AddStaff(string userId, string role, DateTime hireDate)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (Staff.Any(s => s.UserId == userId && s.Role == role))
            throw new InvalidOperationException($"User {userId} already has role {role} in this agency");

        var staff = AgencyStaff.Create(Id, userId, role, hireDate);
        Staff.Add(staff);
        UpdateTimestamp();
    }

    public void RemoveStaff(string userId, string role)
    {
        var staff = Staff.FirstOrDefault(s => s.UserId == userId && s.Role == role);
        if (staff != null)
        {
            Staff.Remove(staff);
            UpdateTimestamp();
        }
    }

    public void AddInterpreter(string userId, List<string> skills)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (Interpreters.Any(i => i.UserId == userId))
            throw new InvalidOperationException($"User {userId} is already an interpreter in this agency");

        var interpreter = Interpreter.Create(Id, userId, skills);
        Interpreters.Add(interpreter);
        UpdateTimestamp();
    }

    public void AddClient(string userId, string organizationName, Dictionary<string, string> preferences)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (Clients.Any(c => c.UserId == userId))
            throw new InvalidOperationException($"User {userId} is already a client in this agency");

        var client = Client.Create(Id, userId, organizationName, preferences);
        Clients.Add(client);
        UpdateTimestamp();
    }
}

public enum AgencyStatus
{
    Active,
    Inactive,
    Suspended
}
