using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.DTOs;
using ThatInterpretingAgency.Core.Domain.Aggregates;

namespace ThatInterpretingAgency.Core.Application.Queries.GetAgency;

public class GetAgencyQueryHandler : IRequestHandler<GetAgencyQuery, AgencyData?>
{
    private readonly IAgencyRepository _agencyRepository;

    public GetAgencyQueryHandler(IAgencyRepository agencyRepository)
    {
        _agencyRepository = agencyRepository;
    }

    public async Task<AgencyData?> Handle(GetAgencyQuery request, CancellationToken cancellationToken)
    {
        var agency = await _agencyRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (agency == null)
            return null;

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
