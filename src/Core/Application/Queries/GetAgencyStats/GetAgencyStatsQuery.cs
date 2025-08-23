using MediatR;
using ThatInterpretingAgency.Core.DTOs;

namespace ThatInterpretingAgency.Core.Application.Queries.GetAgencyStats;

public record GetAgencyStatsQuery : IRequest<AgencyStats>
{
    public Guid Id { get; init; }
}
