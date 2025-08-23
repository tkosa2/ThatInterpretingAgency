using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ThatInterpretingAgency.Core.Application.Common;

namespace ThatInterpretingAgency.Core.Application.Commands.BookAppointment;

public class BookAppointmentCommandHandler : IRequestHandler<BookAppointmentCommand, BookAppointmentResponse>
{
    private readonly IAgencyRepository _agencyRepository;
    private readonly IInterpreterRepository _interpreterRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public BookAppointmentCommandHandler(
        IAgencyRepository agencyRepository,
        IInterpreterRepository interpreterRepository,
        IClientRepository clientRepository,
        IAppointmentRepository appointmentRepository)
    {
        _agencyRepository = agencyRepository;
        _interpreterRepository = interpreterRepository;
        _clientRepository = clientRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<BookAppointmentResponse> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
    {
        // Verify agency exists
        var agency = await _agencyRepository.GetByIdAsync(request.AgencyId, cancellationToken);

        if (agency == null)
        {
            throw new ArgumentException($"Agency with ID '{request.AgencyId}' not found");
        }

        // Verify interpreter exists and belongs to the agency
        var interpreter = await _interpreterRepository.GetByAgencyAndUserIdAsync(request.AgencyId, request.InterpreterId, cancellationToken);

        if (interpreter == null)
        {
            throw new ArgumentException($"Interpreter with ID '{request.InterpreterId}' not found in agency '{request.AgencyId}'");
        }

        // Verify client exists and belongs to the agency
        var client = await _clientRepository.GetByAgencyAndUserIdAsync(request.AgencyId, request.ClientId, cancellationToken);

        if (client == null)
        {
            throw new ArgumentException($"Client with ID '{request.ClientId}' not found in agency '{request.AgencyId}'");
        }

        // Check for overlapping appointments for the interpreter
        var hasOverlapping = await _appointmentRepository.HasOverlappingAppointmentsAsync(
            request.InterpreterId, request.StartTime, request.EndTime, cancellationToken);

        if (hasOverlapping)
        {
            throw new InvalidOperationException($"Interpreter has a conflicting appointment during the requested time slot");
        }

        // Create new appointment
        var appointment = ThatInterpretingAgency.Core.Domain.Aggregates.AppointmentAggregate.Create(
            request.AgencyId,
            request.InterpreterId,
            request.ClientId,
            request.StartTime,
            request.EndTime,
            request.TimeZone,
            request.Location,
            request.Language,
            request.Rate,
            request.Notes
        );

        // Add to repository
        var savedAppointment = await _appointmentRepository.AddAsync(appointment, cancellationToken);

        // Return response
        return new BookAppointmentResponse
        {
            AppointmentId = savedAppointment.Id,
            AgencyId = savedAppointment.AgencyId,
            InterpreterId = savedAppointment.InterpreterId,
            ClientId = savedAppointment.ClientId,
            StartTime = savedAppointment.StartTime,
            EndTime = savedAppointment.EndTime,
            TimeZone = savedAppointment.TimeZone,
            Status = savedAppointment.Status.ToString(),
            CreatedAt = savedAppointment.CreatedAt
        };
    }
}
