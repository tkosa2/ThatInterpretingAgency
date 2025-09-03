using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.Domain.Entities;

namespace ThatInterpretingAgency.Infrastructure.Persistence.Repositories;

public class InterpreterRepository : IInterpreterRepository
{
    private readonly ThatInterpretingAgencyDbContext _context;

    public InterpreterRepository(ThatInterpretingAgencyDbContext context)
    {
        _context = context;
    }

    public async Task<Interpreter?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Interpreters
            .Include(i => i.AvailabilitySlots)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Interpreter>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Interpreters
            .Include(i => i.AvailabilitySlots)
            .ToListAsync(cancellationToken);
    }

    public async Task<Interpreter> AddAsync(Interpreter entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Interpreters.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(Interpreter entity, CancellationToken cancellationToken = default)
    {
        _context.Interpreters.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Interpreter entity, CancellationToken cancellationToken = default)
    {
        _context.Interpreters.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Interpreter>> GetByAgencyIdAsync(Guid agencyId, CancellationToken cancellationToken = default)
    {
        return await _context.Interpreters
            .Include(i => i.AvailabilitySlots)
            .Where(i => i.AgencyId == agencyId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Interpreter?> GetByAgencyAndUserIdAsync(Guid agencyId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Interpreters
            .Include(i => i.AvailabilitySlots)
            .FirstOrDefaultAsync(i => i.AgencyId == agencyId && i.UserId == userId, cancellationToken);
    }
}
