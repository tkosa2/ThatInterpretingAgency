using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.Domain.Aggregates;

namespace ThatInterpretingAgency.Infrastructure.Persistence.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly ThatInterpretingAgencyDbContext _context;

    public InvoiceRepository(ThatInterpretingAgencyDbContext context)
    {
        _context = context;
    }

    public async Task<InvoiceAggregate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<InvoiceAggregate>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Invoices.ToListAsync(cancellationToken);
    }

    public async Task<InvoiceAggregate> AddAsync(InvoiceAggregate entity, CancellationToken cancellationToken = default)
    {
        var entry = await _context.Invoices.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }

    public async Task UpdateAsync(InvoiceAggregate entity, CancellationToken cancellationToken = default)
    {
        _context.Invoices.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(InvoiceAggregate entity, CancellationToken cancellationToken = default)
    {
        _context.Invoices.Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<InvoiceAggregate?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .FirstOrDefaultAsync(i => i.AppointmentId == appointmentId, cancellationToken);
    }
}
