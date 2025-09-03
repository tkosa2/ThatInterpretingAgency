using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class UserCalendarConnection : Entity
{
    public string UserId { get; private set; } = string.Empty;
    public Guid ProviderId { get; private set; }
    public string? ExternalCalendarId { get; private set; }
    public string CalendarName { get; private set; } = string.Empty;
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public DateTime? TokenExpiresAt { get; private set; }
    public bool IsActive { get; private set; }
    public bool SyncEnabled { get; private set; }
    public DateTime? LastSyncAt { get; private set; }

    // Navigation properties
    public virtual CalendarProvider Provider { get; private set; } = null!;
    public virtual ICollection<CalendarEvent> CalendarEvents { get; private set; } = new List<CalendarEvent>();
    public virtual ICollection<CalendarSyncLog> SyncLogs { get; private set; } = new List<CalendarSyncLog>();

    private UserCalendarConnection() { }

    public static UserCalendarConnection Create(string userId, Guid providerId, string calendarName, string? externalCalendarId = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(calendarName))
            throw new ArgumentException("Calendar name cannot be empty", nameof(calendarName));

        return new UserCalendarConnection
        {
            UserId = userId.Trim(),
            ProviderId = providerId,
            CalendarName = calendarName.Trim(),
            ExternalCalendarId = externalCalendarId?.Trim(),
            IsActive = true,
            SyncEnabled = true
        };
    }

    public void UpdateTokens(string? accessToken, string? refreshToken, DateTime? tokenExpiresAt)
    {
        AccessToken = accessToken?.Trim();
        RefreshToken = refreshToken?.Trim();
        TokenExpiresAt = tokenExpiresAt;
        UpdateTimestamp();
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdateTimestamp();
    }

    public void SetSyncEnabled(bool syncEnabled)
    {
        SyncEnabled = syncEnabled;
        UpdateTimestamp();
    }

    public void UpdateLastSync()
    {
        LastSyncAt = DateTime.UtcNow;
        UpdateTimestamp();
    }

    public bool IsTokenExpired => TokenExpiresAt.HasValue && TokenExpiresAt.Value <= DateTime.UtcNow;
    public bool CanSync => IsActive && SyncEnabled && !IsTokenExpired;


}
