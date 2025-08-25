using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class Client : Entity
{
    public Guid AgencyId { get; private set; }
    public string UserId { get; private set; } = string.Empty; // Changed to string to match AspNetUsers.Id
    public string OrganizationName { get; private set; } = string.Empty;
    public Dictionary<string, string> Preferences { get; private set; } = new();
    public ClientStatus Status { get; private set; }

    // Navigation properties
    public virtual UserProfile UserProfile { get; private set; } = null!;

    private Client() { }

    public static Client Create(Guid agencyId, string userId, string organizationName, Dictionary<string, string> preferences)
    {
        if (string.IsNullOrWhiteSpace(organizationName))
            throw new ArgumentException("Organization name cannot be empty", nameof(organizationName));

        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (preferences == null)
            preferences = new Dictionary<string, string>();

        return new Client
        {
            AgencyId = agencyId,
            UserId = userId.Trim(),
            OrganizationName = organizationName.Trim(),
            Preferences = preferences,
            Status = ClientStatus.Active
        };
    }

    public void UpdateOrganizationInfo(string organizationName)
    {
        if (string.IsNullOrWhiteSpace(organizationName))
            throw new ArgumentException("Organization name cannot be empty", nameof(organizationName));

        OrganizationName = organizationName.Trim();
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
