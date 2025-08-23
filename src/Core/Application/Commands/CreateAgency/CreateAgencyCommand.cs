using System;
using MediatR;

namespace ThatInterpretingAgency.Core.Application.Commands.CreateAgency;

public record CreateAgencyCommand : IRequest<CreateAgencyResponse>
{
    public string Name { get; init; } = string.Empty;
    public string ContactInfo { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

public record CreateAgencyResponse
{
    public Guid AgencyId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string ContactInfo { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
