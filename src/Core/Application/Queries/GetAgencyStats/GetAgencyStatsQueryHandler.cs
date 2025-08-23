using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.DTOs;
using ThatInterpretingAgency.Core.Domain.Aggregates;

namespace ThatInterpretingAgency.Core.Application.Queries.GetAgencyStats;

public class GetAgencyStatsQueryHandler : IRequestHandler<GetAgencyStatsQuery, AgencyStats>
{
    private readonly IAgencyRepository _agencyRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IInvoiceRepository _invoiceRepository;

    public GetAgencyStatsQueryHandler(
        IAgencyRepository agencyRepository,
        IAppointmentRepository appointmentRepository,
        IInvoiceRepository invoiceRepository)
    {
        _agencyRepository = agencyRepository;
        _appointmentRepository = appointmentRepository;
        _invoiceRepository = invoiceRepository;
    }

    public async Task<AgencyStats> Handle(GetAgencyStatsQuery request, CancellationToken cancellationToken)
    {
        var agency = await _agencyRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (agency == null)
            throw new ArgumentException($"Agency with ID {request.Id} not found");

        // Get appointments for the agency
        var appointments = await _appointmentRepository.GetByAgencyIdAsync(request.Id, cancellationToken);
        
        // Calculate stats
        var totalStaff = agency.Staff.Count;
        var activeInterpreters = agency.Interpreters.Count(i => i.Status.ToString() == "Active");
        var totalClients = agency.Clients.Count;
        var totalAppointments = appointments.Count();
        
        // Calculate monthly revenue (last 30 days)
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var recentAppointments = appointments.Where(a => a.StartTime >= thirtyDaysAgo);
        var monthlyRevenue = recentAppointments.Sum(a => a.Rate ?? 0);
        
        // Count pending invoices (this would need to be implemented based on your invoice logic)
        var pendingInvoices = 0; // TODO: Implement actual invoice counting logic

        return new AgencyStats
        {
            TotalStaff = totalStaff,
            ActiveInterpreters = activeInterpreters,
            TotalClients = totalClients,
            MonthlyRevenue = monthlyRevenue,
            TotalAppointments = totalAppointments,
            PendingInvoices = pendingInvoices
        };
    }
}
