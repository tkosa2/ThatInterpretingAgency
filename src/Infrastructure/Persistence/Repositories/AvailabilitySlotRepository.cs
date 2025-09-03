using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Domain.Entities;

namespace ThatInterpretingAgency.Infrastructure.Persistence.Repositories;

public class AvailabilitySlotRepository : IAvailabilitySlotRepository
{
    private readonly ThatInterpretingAgencyDbContext _context;

    public AvailabilitySlotRepository(ThatInterpretingAgencyDbContext context)
    {
        _context = context;
    }

    public async Task<AvailabilitySlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AvailabilitySlots
            .Include(a => a.Interpreter)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AvailabilitySlot>> GetByInterpreterIdAsync(Guid interpreterId, CancellationToken cancellationToken = default)
    {
        return await _context.AvailabilitySlots
            .Where(a => a.InterpreterId == interpreterId)
            .OrderBy(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AvailabilitySlot>> GetAvailableSlotsAsync(Guid interpreterId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return await _context.AvailabilitySlots
            .Where(a => a.InterpreterId == interpreterId &&
                       a.Status == AvailabilityStatus.Available &&
                       a.StartTime <= startTime &&
                       a.EndTime >= endTime)
            .OrderBy(a => a.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<AvailabilitySlot> AddAsync(AvailabilitySlot entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.AvailabilitySlots.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(AvailabilitySlot entity, CancellationToken cancellationToken = default)
    {
        _context.AvailabilitySlots.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AvailabilitySlot entity, CancellationToken cancellationToken = default)
    {
        _context.AvailabilitySlots.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingSlotsAsync(Guid interpreterId, DateTime startTime, DateTime endTime, Guid? excludeSlotId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.AvailabilitySlots
            .Where(a => a.InterpreterId == interpreterId &&
                       a.StartTime < endTime &&
                       a.EndTime > startTime);

        if (excludeSlotId.HasValue)
        {
            query = query.Where(a => a.Id != excludeSlotId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
