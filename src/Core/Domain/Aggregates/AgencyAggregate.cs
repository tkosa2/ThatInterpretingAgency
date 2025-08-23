using ThatInterpretingAgency.Core.Domain.Common;
using ThatInterpretingAgency.Core.Domain.Entities;

namespace ThatInterpretingAgency.Core.Domain.Aggregates;

public class AgencyAggregate : AggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string ContactInfo { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public string Phone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public AgencyStatus Status { get; private set; }
    public List<AgencyStaff> Staff { get; private set; } = new();
    public List<Interpreter> Interpreters { get; private set; } = new();
    public List<Client> Clients { get; private set; } = new();

    private AgencyAggregate() { }

    public static AgencyAggregate Create(string name, string contactInfo, string address = "", string phone = "", string email = "")
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Agency name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(contactInfo))
            throw new ArgumentException("Contact info cannot be empty", nameof(contactInfo));

        var agency = new AgencyAggregate
        {
            Name = name.Trim(),
            ContactInfo = contactInfo.Trim(),
            Address = address?.Trim() ?? string.Empty,
            Phone = phone?.Trim() ?? string.Empty,
            Email = email?.Trim() ?? string.Empty,
            Status = AgencyStatus.Active
        };

        return agency;
    }

    public void UpdateContactInfo(string contactInfo)
    {
        if (string.IsNullOrWhiteSpace(contactInfo))
            throw new ArgumentException("Contact info cannot be empty", nameof(contactInfo));

        ContactInfo = contactInfo.Trim();
        UpdateTimestamp();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Agency name cannot be empty", nameof(name));

        Name = name.Trim();
        UpdateTimestamp();
    }

    public void UpdateAddress(string address)
    {
        Address = address?.Trim() ?? string.Empty;
        UpdateTimestamp();
    }

    public void UpdatePhone(string phone)
    {
        Phone = phone?.Trim() ?? string.Empty;
        UpdateTimestamp();
    }

    public void UpdateEmail(string email)
    {
        Email = email?.Trim() ?? string.Empty;
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

    public void AddStaff(Guid userId, string role, DateTime hireDate)
    {
        if (Staff.Any(s => s.UserId == userId && s.Role == role))
            throw new InvalidOperationException($"User {userId} already has role {role} in this agency");

        var staff = AgencyStaff.Create(Id, userId, role, hireDate);
        Staff.Add(staff);
        UpdateTimestamp();
    }

    public void RemoveStaff(Guid userId, string role)
    {
        var staff = Staff.FirstOrDefault(s => s.UserId == userId && s.Role == role);
        if (staff != null)
        {
            Staff.Remove(staff);
            UpdateTimestamp();
        }
    }

    public void AddInterpreter(Guid userId, string fullName, List<string> skills)
    {
        if (Interpreters.Any(i => i.UserId == userId))
            throw new InvalidOperationException($"User {userId} is already an interpreter in this agency");

        var interpreter = Interpreter.Create(Id, userId, fullName, skills);
        Interpreters.Add(interpreter);
        UpdateTimestamp();
    }

    public void AddClient(Guid userId, string organizationName, Dictionary<string, string> preferences)
    {
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
