using MediatR;
using ThatInterpretingAgency.Core.DTOs;

namespace ThatInterpretingAgency.Core.Application.Queries.GetAgency;

public record GetAgencyQuery : IRequest<AgencyData?>
{
    public Guid Id { get; init; }
}
