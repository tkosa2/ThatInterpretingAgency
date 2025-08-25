using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ThatInterpretingAgency.Core.Application.Common;

namespace ThatInterpretingAgency.Core.Application.Commands.CreateInvoice;

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, CreateInvoiceResponse>
{
    private readonly IAgencyRepository _agencyRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IQuickBooksService _quickBooksService;

    public CreateInvoiceCommandHandler(
        IAgencyRepository agencyRepository,
        IClientRepository clientRepository,
        IAppointmentRepository appointmentRepository,
        IInvoiceRepository invoiceRepository,
        IQuickBooksService quickBooksService)
    {
        _agencyRepository = agencyRepository;
        _clientRepository = clientRepository;
        _appointmentRepository = appointmentRepository;
        _invoiceRepository = invoiceRepository;
        _quickBooksService = quickBooksService;
    }

    public async Task<CreateInvoiceResponse> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        // Verify agency exists
        var agency = await _agencyRepository.GetByIdAsync(request.AgencyId, cancellationToken);

        if (agency == null)
        {
            throw new ArgumentException($"Agency with ID '{request.AgencyId}' not found");
        }

        // Verify client exists and belongs to the agency
        var client = await _clientRepository.GetByAgencyAndUserIdAsync(request.AgencyId, request.ClientId, cancellationToken);

        if (client == null)
        {
            throw new ArgumentException($"Client with ID '{request.ClientId}' not found in agency '{request.AgencyId}'");
        }

        // Verify appointment exists and belongs to the agency
        var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

        if (appointment == null || appointment.AgencyId != request.AgencyId)
        {
            throw new ArgumentException($"Appointment with ID '{request.AppointmentId}' not found in agency '{request.AgencyId}'");
        }

        // Check if invoice already exists for this appointment
        var existingInvoice = await _invoiceRepository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);

        if (existingInvoice != null)
        {
            throw new InvalidOperationException($"Invoice already exists for appointment '{request.AppointmentId}'");
        }

        // Create QuickBooks invoice
        var quickBooksRequest = new CreateInvoiceRequest
        {
            ClientId = request.ClientId,
            AppointmentId = request.AppointmentId,
            ClientName = client.OrganizationName,
            ClientEmail = string.Empty, // This will come from UserProfile when we have users
            AppointmentDate = appointment.StartTime,
            Duration = appointment.Duration,
            Amount = request.Amount ?? 0,
            Currency = request.Currency ?? "USD",
            Description = request.Notes ?? $"Interpreting services on {appointment.StartTime:yyyy-MM-dd}"
        };

        var quickBooksInvoiceId = await _quickBooksService.CreateInvoiceAsync(quickBooksRequest, cancellationToken);

        // Create local invoice record
        var invoice = ThatInterpretingAgency.Core.Domain.Aggregates.InvoiceAggregate.Create(
            request.AgencyId,
            request.ClientId,
            request.AppointmentId,
            quickBooksInvoiceId,
            request.DueDate,
            request.Amount,
            request.Currency,
            request.Notes
        );

        // Add to repository
        var savedInvoice = await _invoiceRepository.AddAsync(invoice, cancellationToken);

        // Return response
        return new CreateInvoiceResponse
        {
            InvoiceId = savedInvoice.Id,
            AgencyId = savedInvoice.AgencyId,
            ClientId = savedInvoice.ClientId,
            AppointmentId = savedInvoice.AppointmentId,
            QuickBooksInvoiceId = savedInvoice.QuickBooksInvoiceId,
            Status = savedInvoice.Status.ToString(),
            CreatedAt = savedInvoice.CreatedAt
        };
    }
}
