using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.Domain.Entities;

namespace ThatInterpretingAgency.Infrastructure.Persistence.Repositories;

public class InterpreterRequestRepository : IInterpreterRequestRepository
{
    private readonly ThatInterpretingAgencyDbContext _context;

    public InterpreterRequestRepository(ThatInterpretingAgencyDbContext context)
    {
        _context = context;
    }

    public async Task<InterpreterRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.InterpreterRequests
            .Include(ir => ir.Agency)
            .Include(ir => ir.Requestor)
            .Include(ir => ir.Appointment)
            .FirstOrDefaultAsync(ir => ir.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<InterpreterRequest>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InterpreterRequests
            .Include(ir => ir.Agency)
            .Include(ir => ir.Requestor)
            .Include(ir => ir.Appointment)
            .OrderByDescending(ir => ir.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InterpreterRequest>> GetByAgencyIdAsync(Guid agencyId, CancellationToken cancellationToken = default)
    {
        return await _context.InterpreterRequests
            .Include(ir => ir.Agency)
            .Include(ir => ir.Requestor)
            .Include(ir => ir.Appointment)
            .Where(ir => ir.AgencyId == agencyId)
            .OrderByDescending(ir => ir.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InterpreterRequest>> GetByRequestorIdAsync(Guid requestorId, CancellationToken cancellationToken = default)
    {
        return await _context.InterpreterRequests
            .Include(ir => ir.Agency)
            .Include(ir => ir.Requestor)
            .Include(ir => ir.Appointment)
            .Where(ir => ir.RequestorId == requestorId)
            .OrderByDescending(ir => ir.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InterpreterRequest>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
    {
        return await _context.InterpreterRequests
            .Include(ir => ir.Agency)
            .Include(ir => ir.Requestor)
            .Include(ir => ir.Appointment)
            .Where(ir => ir.Status == status)
            .OrderByDescending(ir => ir.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InterpreterRequest>> GetByLanguageAsync(string language, CancellationToken cancellationToken = default)
    {
        return await _context.InterpreterRequests
            .Include(ir => ir.Agency)
            .Include(ir => ir.Requestor)
            .Include(ir => ir.Appointment)
            .Where(ir => ir.Language.Contains(language))
            .OrderByDescending(ir => ir.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InterpreterRequest>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.InterpreterRequests
            .Include(ir => ir.Agency)
            .Include(ir => ir.Requestor)
            .Include(ir => ir.Appointment)
            .Where(ir => ir.RequestedDate >= fromDate && ir.RequestedDate <= toDate)
            .OrderByDescending(ir => ir.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InterpreterRequest>> GetApprovedRequestsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.InterpreterRequests
            .Include(ir => ir.Agency)
            .Include(ir => ir.Requestor)
            .Include(ir => ir.Appointment)
            .Where(ir => ir.Status == "Approved")
            .OrderBy(ir => ir.RequestedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<InterpreterRequest> AddAsync(InterpreterRequest interpreterRequest, CancellationToken cancellationToken = default)
    {
        _context.InterpreterRequests.Add(interpreterRequest);
        await _context.SaveChangesAsync(cancellationToken);
        return interpreterRequest;
    }

    public async Task UpdateAsync(InterpreterRequest interpreterRequest, CancellationToken cancellationToken = default)
    {
        _context.InterpreterRequests.Update(interpreterRequest);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(InterpreterRequest interpreterRequest, CancellationToken cancellationToken = default)
    {
        _context.InterpreterRequests.Remove(interpreterRequest);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.InterpreterRequests.AnyAsync(ir => ir.Id == id, cancellationToken);
    }
}
