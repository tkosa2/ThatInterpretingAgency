using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.Domain.Aggregates;

namespace ThatInterpretingAgency.Infrastructure.Persistence.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly ThatInterpretingAgencyDbContext _context;

    public AppointmentRepository(ThatInterpretingAgencyDbContext context)
    {
        _context = context;
    }

    public async Task<AppointmentAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AppointmentAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Appointments.ToListAsync(cancellationToken);
    }

    public async Task<AppointmentAggregate> AddAsync(AppointmentAggregate entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Appointments.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(AppointmentAggregate entity, CancellationToken cancellationToken = default)
    {
        _context.Appointments.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AppointmentAggregate entity, CancellationToken cancellationToken = default)
    {
        _context.Appointments.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<AppointmentAggregate>> GetByAgencyIdAsync(Guid agencyId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Where(a => a.AgencyId == agencyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AppointmentAggregate>> GetByInterpreterIdAsync(string interpreterId, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .Where(a => a.InterpreterId == interpreterId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasOverlappingAppointmentsAsync(string interpreterId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default)
    {
        return await _context.Appointments
            .AnyAsync(a => a.InterpreterId == interpreterId && 
                          a.Status != AppointmentStatus.Cancelled &&
                          a.Status != AppointmentStatus.Completed &&
                          ((a.StartTime <= startTime && a.EndTime > startTime) ||
                           (a.StartTime < endTime && a.EndTime >= endTime) ||
                           (a.StartTime >= startTime && a.EndTime <= endTime)), 
                          cancellationToken);
    }
}
