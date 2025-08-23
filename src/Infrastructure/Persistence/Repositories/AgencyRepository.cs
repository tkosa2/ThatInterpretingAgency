using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.Domain.Aggregates;

namespace ThatInterpretingAgency.Infrastructure.Persistence.Repositories;

public class AgencyRepository : IAgencyRepository
{
    private readonly ThatInterpretingAgencyDbContext _context;

    public AgencyRepository(ThatInterpretingAgencyDbContext context)
    {
        _context = context;
    }

    public async Task<AgencyAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Agencies
            .Include(a => a.Staff)
            .Include(a => a.Interpreters)
            .Include(a => a.Clients)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AgencyAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Agencies
            .Include(a => a.Staff)
            .Include(a => a.Interpreters)
            .Include(a => a.Clients)
            .ToListAsync(cancellationToken);
    }

    public async Task<AgencyAggregate> AddAsync(AgencyAggregate entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Agencies.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(AgencyAggregate entity, CancellationToken cancellationToken = default)
    {
        _context.Agencies.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AgencyAggregate entity, CancellationToken cancellationToken = default)
    {
        _context.Agencies.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AgencyAggregate?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _context.Agencies
            .Include(a => a.Staff)
            .Include(a => a.Interpreters)
            .Include(a => a.Clients)
            .FirstOrDefaultAsync(a => a.Name == name, cancellationToken);
    }
}
