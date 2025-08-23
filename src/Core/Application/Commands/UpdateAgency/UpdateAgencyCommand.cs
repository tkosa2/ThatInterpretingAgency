using MediatR;
using ThatInterpretingAgency.Core.DTOs;

namespace ThatInterpretingAgency.Core.Application.Commands.UpdateAgency;

public record UpdateAgencyCommand : IRequest<AgencyData>
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ContactInfo { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
}
