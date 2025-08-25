using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ThatInterpretingAgency.Core.Application.Common;

namespace ThatInterpretingAgency.Core.Application.Queries.GetAvailableInterpreters;

public class GetAvailableInterpretersQueryHandler : IRequestHandler<GetAvailableInterpretersQuery, GetAvailableInterpretersResponse>
{
    private readonly IInterpreterRepository _interpreterRepository;
    private readonly IAppointmentRepository _appointmentRepository;

    public GetAvailableInterpretersQueryHandler(IInterpreterRepository interpreterRepository, IAppointmentRepository appointmentRepository)
    {
        _interpreterRepository = interpreterRepository;
        _appointmentRepository = appointmentRepository;
    }

    public async Task<GetAvailableInterpretersResponse> Handle(GetAvailableInterpretersQuery request, CancellationToken cancellationToken)
    {
        // Get all active interpreters in the agency
        var interpreters = await _interpreterRepository.GetByAgencyIdAsync(request.AgencyId, cancellationToken);

        // Filter by required skills if specified
        if (request.RequiredSkills != null && request.RequiredSkills.Any())
        {
            interpreters = interpreters.Where(i => request.RequiredSkills.Any(skill => i.Skills.Contains(skill))).ToList();
        }

        // Filter by language if specified
        if (!string.IsNullOrWhiteSpace(request.Language))
        {
            interpreters = interpreters.Where(i => i.Skills.Contains(request.Language)).ToList();
        }

        var availableInterpreters = new List<AvailableInterpreterDto>();

        foreach (var interpreter in interpreters)
        {
            // Check if interpreter has availability during the requested time
            var hasAvailability = await CheckInterpreterAvailability(
                interpreter.UserId, 
                request.StartTime, 
                request.EndTime, 
                cancellationToken);

            if (hasAvailability)
            {
                var availableSlots = await GetAvailableSlots(
                    interpreter.UserId, 
                    request.StartTime, 
                    request.EndTime, 
                    cancellationToken);

                var interpreterDto = new AvailableInterpreterDto
                {
                    Id = interpreter.Id,
                    FullName = string.Empty, // This will come from UserProfile when we have users
                    Skills = interpreter.Skills,
                    AvailableSlots = availableSlots,
                    HourlyRate = null // TODO: Add hourly rate to interpreter entity
                };

                availableInterpreters.Add(interpreterDto);
            }
        }

        return new GetAvailableInterpretersResponse
        {
            Interpreters = availableInterpreters
        };
    }

    private async Task<bool> CheckInterpreterAvailability(string interpreterId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
    {
        // Check for conflicting appointments
        var hasOverlapping = await _appointmentRepository.HasOverlappingAppointmentsAsync(interpreterId, startTime, endTime, cancellationToken);
        return !hasOverlapping;
    }

    private async Task<List<AvailabilitySlotDto>> GetAvailableSlots(string interpreterId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken)
    {
        // For now, return the requested time slot as available
        // TODO: Implement proper availability slot logic
        return new List<AvailabilitySlotDto>
        {
            new AvailabilitySlotDto
            {
                StartTime = startTime,
                EndTime = endTime,
                TimeZone = "UTC"
            }
        };
    }
}
