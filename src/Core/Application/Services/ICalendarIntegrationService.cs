using ThatInterpretingAgency.Core.Domain.Entities;

namespace ThatInterpretingAgency.Core.Application.Services;

public interface ICalendarIntegrationService
{
    // Calendar Provider Management
    Task<IEnumerable<CalendarProvider>> GetActiveProvidersAsync(CancellationToken cancellationToken = default);
    Task<CalendarProvider?> GetProviderByIdAsync(Guid providerId, CancellationToken cancellationToken = default);
    
    // User Calendar Connections
    Task<IEnumerable<UserCalendarConnection>> GetUserConnectionsAsync(string userId, CancellationToken cancellationToken = default);
    Task<UserCalendarConnection?> GetConnectionByIdAsync(Guid connectionId, CancellationToken cancellationToken = default);
    Task<UserCalendarConnection> CreateConnectionAsync(string userId, Guid providerId, string calendarName, string? externalCalendarId = null, CancellationToken cancellationToken = default);
    Task UpdateConnectionTokensAsync(Guid connectionId, string? accessToken, string? refreshToken, DateTime? tokenExpiresAt, CancellationToken cancellationToken = default);
    Task DeleteConnectionAsync(Guid connectionId, CancellationToken cancellationToken = default);
    
    // Calendar Events
    Task<IEnumerable<CalendarEvent>> GetUserEventsAsync(string userId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    Task<IEnumerable<CalendarEvent>> GetEventsByTypeAsync(string userId, string eventType, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    Task<CalendarEvent?> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<CalendarEvent> CreateEventAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken = default);
    Task UpdateEventAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken = default);
    Task DeleteEventAsync(Guid eventId, CancellationToken cancellationToken = default);
    
    // Availability Management
    Task<IEnumerable<CalendarEvent>> GetAvailableSlotsAsync(string userId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    Task<bool> HasConflictsAsync(string userId, DateTime startTime, DateTime endTime, Guid? excludeEventId = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<CalendarEvent>> GetConflictsAsync(string userId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    
    // Third-party Calendar Integration
    Task<bool> SyncWithExternalCalendarAsync(Guid connectionId, CancellationToken cancellationToken = default);
    Task<bool> PushEventToExternalCalendarAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task<bool> PullEventsFromExternalCalendarAsync(Guid connectionId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    
    // Calendar Templates
    Task<IEnumerable<CalendarEvent>> GenerateEventsFromTemplateAsync(Guid templateId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    
    // Utility Methods
    Task<bool> IsTimeSlotAvailableAsync(string userId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);
    Task<TimeSpan> GetTotalAvailabilityAsync(string userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
