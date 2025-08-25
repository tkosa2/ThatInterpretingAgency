using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.Domain.Entities;

namespace ThatInterpretingAgency.Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly ThatInterpretingAgencyDbContext _context;

    public ClientRepository(ThatInterpretingAgencyDbContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Client>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Clients.ToListAsync(cancellationToken);
    }

    public async Task<Client> AddAsync(Client entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Clients.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(Client entity, CancellationToken cancellationToken = default)
    {
        _context.Clients.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Client entity, CancellationToken cancellationToken = default)
    {
        _context.Clients.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Client?> GetByAgencyAndUserIdAsync(Guid agencyId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Clients
            .FirstOrDefaultAsync(c => c.AgencyId == agencyId && c.UserId == userId, cancellationToken);
    }
}
