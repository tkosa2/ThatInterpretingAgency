using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Domain.Entities;
using ThatInterpretingAgency.Infrastructure.Persistence;
using ThatInterpretingAgency.Core.Application.Services;

namespace ThatInterpretingAgency.Infrastructure.Services;

public class CalendarIntegrationService : ICalendarIntegrationService
{
    private readonly ThatInterpretingAgencyDbContext _context;

    public CalendarIntegrationService(ThatInterpretingAgencyDbContext context)
    {
        _context = context;
    }

    // Calendar Provider Management
    public async Task<IEnumerable<CalendarProvider>> GetActiveProvidersAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<CalendarProvider>()
            .Where(p => p.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<CalendarProvider?> GetProviderByIdAsync(Guid providerId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CalendarProvider>()
            .FirstOrDefaultAsync(p => p.Id == providerId, cancellationToken);
    }

    // User Calendar Connections
    public async Task<IEnumerable<UserCalendarConnection>> GetUserConnectionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserCalendarConnection>()
            .Where(c => c.UserId == userId && c.IsActive)
            .Include(c => c.Provider)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserCalendarConnection?> GetConnectionByIdAsync(Guid connectionId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<UserCalendarConnection>()
            .Include(c => c.Provider)
            .FirstOrDefaultAsync(c => c.Id == connectionId, cancellationToken);
    }

    public async Task<UserCalendarConnection> CreateConnectionAsync(string userId, Guid providerId, string calendarName, string? externalCalendarId = null, CancellationToken cancellationToken = default)
    {
        var connection = UserCalendarConnection.Create(userId, providerId, calendarName, externalCalendarId);
        _context.Set<UserCalendarConnection>().Add(connection);
        await _context.SaveChangesAsync(cancellationToken);
        return connection;
    }

    public async Task UpdateConnectionTokensAsync(Guid connectionId, string? accessToken, string? refreshToken, DateTime? tokenExpiresAt, CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionByIdAsync(connectionId, cancellationToken);
        if (connection != null)
        {
            connection.UpdateTokens(accessToken, refreshToken, tokenExpiresAt);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteConnectionAsync(Guid connectionId, CancellationToken cancellationToken = default)
    {
        var connection = await GetConnectionByIdAsync(connectionId, cancellationToken);
        if (connection != null)
        {
            _context.Set<UserCalendarConnection>().Remove(connection);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    // Calendar Events
    public async Task<IEnumerable<CalendarEvent>> GetUserEventsAsync(string userId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CalendarEvent>()
            .Where(e => e.UserId == userId && 
                       e.StartTimeUtc < endTime && 
                       e.EndTimeUtc > startTime &&
                       e.Status == "Active")
            .Include(e => e.Connection)
            .Include(e => e.Attendees)
            .OrderBy(e => e.StartTimeUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CalendarEvent>> GetEventsByTypeAsync(string userId, string eventType, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CalendarEvent>()
            .Where(e => e.UserId == userId && 
                       e.EventType == eventType &&
                       e.StartTimeUtc < endTime && 
                       e.EndTimeUtc > startTime &&
                       e.Status == "Active")
            .Include(e => e.Connection)
            .Include(e => e.Attendees)
            .OrderBy(e => e.StartTimeUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<CalendarEvent?> GetEventByIdAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CalendarEvent>()
            .Include(e => e.Connection)
            .Include(e => e.Attendees)
            .FirstOrDefaultAsync(e => e.Id == eventId, cancellationToken);
    }

    public async Task<CalendarEvent> CreateEventAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken = default)
    {
        _context.Set<CalendarEvent>().Add(calendarEvent);
        await _context.SaveChangesAsync(cancellationToken);
        return calendarEvent;
    }

    public async Task UpdateEventAsync(CalendarEvent calendarEvent, CancellationToken cancellationToken = default)
    {
        _context.Set<CalendarEvent>().Update(calendarEvent);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var calendarEvent = await GetEventByIdAsync(eventId, cancellationToken);
        if (calendarEvent != null)
        {
            _context.Set<CalendarEvent>().Remove(calendarEvent);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    // Availability Management
    public async Task<IEnumerable<CalendarEvent>> GetAvailableSlotsAsync(string userId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CalendarEvent>()
            .Where(e => e.UserId == userId && 
                       e.EventType == "Availability" &&
                       e.StartTimeUtc < endTime && 
                       e.EndTimeUtc > startTime &&
                       e.Status == "Active")
            .OrderBy(e => e.StartTimeUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasConflictsAsync(string userId, DateTime startTime, DateTime endTime, Guid? excludeEventId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Set<CalendarEvent>()
            .Where(e => e.UserId == userId && 
                       e.Status == "Active" &&
                       e.StartTimeUtc < endTime && 
                       e.EndTimeUtc > startTime);

        if (excludeEventId.HasValue)
        {
            query = query.Where(e => e.Id != excludeEventId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<IEnumerable<CalendarEvent>> GetConflictsAsync(string userId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return await _context.Set<CalendarEvent>()
            .Where(e => e.UserId == userId && 
                       e.Status == "Active" &&
                       e.StartTimeUtc < endTime && 
                       e.EndTimeUtc > startTime)
            .Include(e => e.Connection)
            .OrderBy(e => e.StartTimeUtc)
            .ToListAsync(cancellationToken);
    }

    // Third-party Calendar Integration (Basic implementation for now)
    public async Task<bool> SyncWithExternalCalendarAsync(Guid connectionId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual sync logic
        var connection = await GetConnectionByIdAsync(connectionId, cancellationToken);
        if (connection?.CanSync == true)
        {
            connection.UpdateLastSync();
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        return false;
    }

    public async Task<bool> PushEventToExternalCalendarAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual push logic
        var calendarEvent = await GetEventByIdAsync(eventId, cancellationToken);
        return calendarEvent != null;
    }

    public async Task<bool> PullEventsFromExternalCalendarAsync(Guid connectionId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        // TODO: Implement actual pull logic
        var connection = await GetConnectionByIdAsync(connectionId, cancellationToken);
        if (connection?.CanSync == true)
        {
            connection.UpdateLastSync();
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
        return false;
    }

    // Calendar Templates (Basic implementation for now)
    public async Task<IEnumerable<CalendarEvent>> GenerateEventsFromTemplateAsync(Guid templateId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        // TODO: Implement template-based event generation
        return new List<CalendarEvent>();
    }

    // Utility Methods
    public async Task<bool> IsTimeSlotAvailableAsync(string userId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return !await HasConflictsAsync(userId, startTime, endTime, null, cancellationToken);
    }

    public async Task<TimeSpan> GetTotalAvailabilityAsync(string userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var availableSlots = await GetAvailableSlotsAsync(userId, startDate, endDate, cancellationToken);
        var totalTime = TimeSpan.Zero;
        
        foreach (var slot in availableSlots)
        {
            var slotStart = slot.StartTimeUtc < startDate ? startDate : slot.StartTimeUtc;
            var slotEnd = slot.EndTimeUtc > endDate ? endDate : slot.EndTimeUtc;
            if (slotStart < slotEnd)
            {
                totalTime += slotEnd - slotStart;
            }
        }
        
        return totalTime;
    }
}
