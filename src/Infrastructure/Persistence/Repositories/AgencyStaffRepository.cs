using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.Domain.Entities;
using ThatInterpretingAgency.Infrastructure.Persistence;

namespace ThatInterpretingAgency.Infrastructure.Persistence.Repositories;

public class AgencyStaffRepository : IAgencyStaffRepository
{
    private readonly ThatInterpretingAgencyDbContext _context;

    public AgencyStaffRepository(ThatInterpretingAgencyDbContext context)
    {
        _context = context;
    }

    public async Task<AgencyStaff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AgencyStaff
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<AgencyStaff>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.AgencyStaff
            .OrderBy(s => s.HireDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<AgencyStaff>> GetByAgencyIdAsync(Guid agencyId, CancellationToken cancellationToken = default)
    {
        return await _context.AgencyStaff
            .Where(s => s.AgencyId == agencyId)
            .OrderBy(s => s.HireDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<AgencyStaff?> GetByAgencyAndUserIdAsync(Guid agencyId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.AgencyStaff
            .FirstOrDefaultAsync(s => s.AgencyId == agencyId && s.UserId == userId, cancellationToken);
    }

    public async Task<AgencyStaff> AddAsync(AgencyStaff entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.AgencyStaff.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(AgencyStaff entity, CancellationToken cancellationToken = default)
    {
        _context.AgencyStaff.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(AgencyStaff entity, CancellationToken cancellationToken = default)
    {
        _context.AgencyStaff.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.AgencyStaff
            .AnyAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<bool> UserHasRoleInAgencyAsync(Guid userId, Guid agencyId, string role, CancellationToken cancellationToken = default)
    {
        return await _context.AgencyStaff
            .AnyAsync(s => s.UserId == userId && s.AgencyId == agencyId && s.Role == role, cancellationToken);
    }
}
