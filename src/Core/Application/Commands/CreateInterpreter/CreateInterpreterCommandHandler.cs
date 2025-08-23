using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ThatInterpretingAgency.Core.Application.Common;

namespace ThatInterpretingAgency.Core.Application.Commands.CreateInterpreter;

public class CreateInterpreterCommandHandler : IRequestHandler<CreateInterpreterCommand, CreateInterpreterResponse>
{
    private readonly IAgencyRepository _agencyRepository;
    private readonly IInterpreterRepository _interpreterRepository;

    public CreateInterpreterCommandHandler(IAgencyRepository agencyRepository, IInterpreterRepository interpreterRepository)
    {
        _agencyRepository = agencyRepository;
        _interpreterRepository = interpreterRepository;
    }

    public async Task<CreateInterpreterResponse> Handle(CreateInterpreterCommand request, CancellationToken cancellationToken)
    {
        // Verify agency exists
        var agency = await _agencyRepository.GetByIdAsync(request.AgencyId, cancellationToken);

        if (agency == null)
        {
            throw new ArgumentException($"Agency with ID '{request.AgencyId}' not found");
        }

        // Check if user is already an interpreter in this agency
        var existingInterpreter = await _interpreterRepository.GetByAgencyAndUserIdAsync(request.AgencyId, request.UserId, cancellationToken);

        if (existingInterpreter != null)
        {
            throw new InvalidOperationException($"User {request.UserId} is already an interpreter in agency {request.AgencyId}");
        }

        // Create new interpreter
        var interpreter = ThatInterpretingAgency.Core.Domain.Entities.Interpreter.Create(request.AgencyId, request.UserId, request.FullName, request.Skills);

        // Add to repository
        var savedInterpreter = await _interpreterRepository.AddAsync(interpreter, cancellationToken);

        // Return response
        return new CreateInterpreterResponse
        {
            InterpreterId = savedInterpreter.Id,
            AgencyId = savedInterpreter.AgencyId,
            UserId = savedInterpreter.UserId,
            FullName = savedInterpreter.FullName,
            Skills = savedInterpreter.Skills,
            CreatedAt = savedInterpreter.CreatedAt
        };
    }
}
