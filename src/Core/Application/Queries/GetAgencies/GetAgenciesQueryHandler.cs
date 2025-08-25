using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.DTOs;
using ThatInterpretingAgency.Core.Domain.Aggregates;

namespace ThatInterpretingAgency.Core.Application.Queries.GetAgencies;

public class GetAgenciesQueryHandler : IRequestHandler<GetAgenciesQuery, IEnumerable<AgencyData>>
{
    private readonly IAgencyRepository _agencyRepository;

    public GetAgenciesQueryHandler(IAgencyRepository agencyRepository)
    {
        _agencyRepository = agencyRepository;
    }

    public async Task<IEnumerable<AgencyData>> Handle(GetAgenciesQuery request, CancellationToken cancellationToken)
    {
        var agencies = await _agencyRepository.GetAllAsync(cancellationToken);
        
        return agencies.Select(agency => new AgencyData
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
        });
    }
}
