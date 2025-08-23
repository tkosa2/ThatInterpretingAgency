using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class Client : Entity
{
    public Guid AgencyId { get; private set; }
    public Guid UserId { get; private set; }
    public string OrganizationName { get; private set; } = string.Empty;
    public Dictionary<string, string> Preferences { get; private set; } = new();
    public ClientStatus Status { get; private set; }
    public string? ContactPerson { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Email { get; private set; }

    private Client() { }

    public static Client Create(Guid agencyId, Guid userId, string organizationName, Dictionary<string, string> preferences)
    {
        if (string.IsNullOrWhiteSpace(organizationName))
            throw new ArgumentException("Organization name cannot be empty", nameof(organizationName));

        if (preferences == null)
            preferences = new Dictionary<string, string>();

        return new Client
        {
            AgencyId = agencyId,
            UserId = userId,
            OrganizationName = organizationName.Trim(),
            Preferences = preferences,
            Status = ClientStatus.Active
        };
    }

    public void UpdateOrganizationInfo(string organizationName, string? contactPerson = null, string? phoneNumber = null, string? email = null)
    {
        if (string.IsNullOrWhiteSpace(organizationName))
            throw new ArgumentException("Organization name cannot be empty", nameof(organizationName));

        OrganizationName = organizationName.Trim();
        ContactPerson = contactPerson?.Trim();
        PhoneNumber = phoneNumber?.Trim();
        Email = email?.Trim();
        UpdateTimestamp();
    }

    public void UpdatePreferences(Dictionary<string, string> newPreferences)
    {
        Preferences = newPreferences ?? new Dictionary<string, string>();
        UpdateTimestamp();
    }

    public void AddPreference(string key, string value)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Preference key cannot be empty", nameof(key));

        Preferences[key.Trim()] = value?.Trim() ?? string.Empty;
        UpdateTimestamp();
    }

    public void RemovePreference(string key)
    {
        if (Preferences.Remove(key))
        {
            UpdateTimestamp();
        }
    }

    public void UpdateStatus(ClientStatus newStatus)
    {
        Status = newStatus;
        UpdateTimestamp();
    }

    public string? GetPreference(string key)
    {
        return Preferences.TryGetValue(key, out var value) ? value : null;
    }
}

public enum ClientStatus
{
    Active,
    Inactive,
    Suspended,
    Blacklisted
}
