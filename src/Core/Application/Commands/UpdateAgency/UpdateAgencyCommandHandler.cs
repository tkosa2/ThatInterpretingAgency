using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.DTOs;
using ThatInterpretingAgency.Core.Domain.Aggregates;

namespace ThatInterpretingAgency.Core.Application.Commands.UpdateAgency;

public class UpdateAgencyCommandHandler : IRequestHandler<UpdateAgencyCommand, AgencyData>
{
    private readonly IAgencyRepository _agencyRepository;
    private readonly IAgencyUniquenessService _uniquenessService;

    public UpdateAgencyCommandHandler(IAgencyRepository agencyRepository, IAgencyUniquenessService uniquenessService)
    {
        _agencyRepository = agencyRepository;
        _uniquenessService = uniquenessService;
    }

    public async Task<AgencyData> Handle(UpdateAgencyCommand request, CancellationToken cancellationToken)
    {
        var agency = await _agencyRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (agency == null)
            throw new ArgumentException($"Agency with ID {request.Id} not found");

        // Check if name is being updated and if the new name already exists
        if (!string.IsNullOrWhiteSpace(request.Name) && !request.Name.Equals(agency.Name, StringComparison.OrdinalIgnoreCase))
        {
            var isNameUnique = await _uniquenessService.IsNameUniqueAsync(request.Name, request.Id, cancellationToken);
            if (!isNameUnique)
            {
                throw new InvalidOperationException($"Agency with name '{request.Name}' already exists");
            }
        }

        // Update the agency properties
        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            agency.UpdateName(request.Name);
        }

        // Update description
        if (!string.IsNullOrWhiteSpace(request.ContactInfo))
        {
            agency.UpdateDescription(request.ContactInfo);
        }

        // Update status if provided
        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (request.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                agency.Activate();
            else if (request.Status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
                agency.Deactivate();
        }

        await _agencyRepository.UpdateAsync(agency, cancellationToken);

        return new AgencyData
        {
            Id = agency.Id.ToString(),
            Name = agency.Name,
            ContactInfo = agency.Description ?? string.Empty,
            Address = string.Empty, // These will come from UserProfile when we have users
            Phone = string.Empty,
            Email = string.Empty,
            Status = agency.Status.ToString(),
            CreatedAt = agency.CreatedAt,
            UpdatedAt = agency.UpdatedAt
        };
    }
}
