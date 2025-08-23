using MediatR;
using ThatInterpretingAgency.Core.DTOs;

namespace ThatInterpretingAgency.Core.Application.Queries.GetAgencies;

public record GetAgenciesQuery : IRequest<IEnumerable<AgencyData>>
{
}
