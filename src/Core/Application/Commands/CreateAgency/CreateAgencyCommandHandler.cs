using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ThatInterpretingAgency.Core.Application.Common;

namespace ThatInterpretingAgency.Core.Application.Commands.CreateAgency;

public class CreateAgencyCommandHandler : IRequestHandler<CreateAgencyCommand, CreateAgencyResponse>
{
    private readonly IAgencyRepository _agencyRepository;
    private readonly IAgencyUniquenessService _uniquenessService;

    public CreateAgencyCommandHandler(IAgencyRepository agencyRepository, IAgencyUniquenessService uniquenessService)
    {
        _agencyRepository = agencyRepository;
        _uniquenessService = uniquenessService;
    }

    public async Task<CreateAgencyResponse> Handle(CreateAgencyCommand request, CancellationToken cancellationToken)
    {
        // Check if agency with the same name already exists
        var isNameUnique = await _uniquenessService.IsNameUniqueAsync(request.Name, null, cancellationToken);

        if (!isNameUnique)
        {
            throw new InvalidOperationException($"Agency with name '{request.Name}' already exists");
        }

        // Create new agency
        var agency = ThatInterpretingAgency.Core.Domain.Aggregates.AgencyAggregate.Create(
            request.Name, 
            request.ContactInfo, 
            request.Address, 
            request.Phone, 
            request.Email);

        // Add to repository
        var savedAgency = await _agencyRepository.AddAsync(agency, cancellationToken);

        // Return response
        return new CreateAgencyResponse
        {
            AgencyId = savedAgency.Id,
            Name = savedAgency.Name,
            ContactInfo = savedAgency.ContactInfo,
            Address = savedAgency.Address,
            Phone = savedAgency.Phone,
            Email = savedAgency.Email,
            CreatedAt = savedAgency.CreatedAt
        };
    }
}
