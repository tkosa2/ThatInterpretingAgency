using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class CalendarProvider : Entity
{
    public string Name { get; private set; } = string.Empty;
    public string ProviderType { get; private set; } = string.Empty; // 'Outlook', 'Gmail', 'iCal', 'Custom'
    public string? ApiEndpoint { get; private set; }
    public string? ClientId { get; private set; }
    public string? ClientSecret { get; private set; }
    public string? Scopes { get; private set; }
    public bool IsActive { get; private set; }

    // Navigation properties
    public virtual ICollection<UserCalendarConnection> UserConnections { get; private set; } = new List<UserCalendarConnection>();

    private CalendarProvider() { }

    public static CalendarProvider Create(string name, string providerType, string? apiEndpoint = null, string? clientId = null, string? clientSecret = null, string? scopes = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(providerType))
            throw new ArgumentException("Provider type cannot be empty", nameof(providerType));

        return new CalendarProvider
        {
            Name = name.Trim(),
            ProviderType = providerType.Trim(),
            ApiEndpoint = apiEndpoint?.Trim(),
            ClientId = clientId?.Trim(),
            ClientSecret = clientSecret?.Trim(),
            Scopes = scopes?.Trim(),
            IsActive = true
        };
    }

    public void UpdateConfiguration(string? apiEndpoint, string? clientId, string? clientSecret, string? scopes)
    {
        ApiEndpoint = apiEndpoint?.Trim();
        ClientId = clientId?.Trim();
        ClientSecret = clientSecret?.Trim();
        Scopes = scopes?.Trim();
        UpdateTimestamp();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdateTimestamp();
    }

    public bool IsOutlookProvider => ProviderType.Equals("Outlook", StringComparison.OrdinalIgnoreCase);
    public bool IsGmailProvider => ProviderType.Equals("Gmail", StringComparison.OrdinalIgnoreCase);
    public bool IsICalProvider => ProviderType.Equals("iCal", StringComparison.OrdinalIgnoreCase);
    public bool IsCustomProvider => ProviderType.Equals("Custom", StringComparison.OrdinalIgnoreCase);


}
