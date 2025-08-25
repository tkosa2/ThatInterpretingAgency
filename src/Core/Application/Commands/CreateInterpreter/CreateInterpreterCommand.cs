using System;
using System.Collections.Generic;
using MediatR;

namespace ThatInterpretingAgency.Core.Application.Commands.CreateInterpreter;

public record CreateInterpreterCommand : IRequest<CreateInterpreterResponse>
{
    public Guid AgencyId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public List<string> Skills { get; init; } = new();
}

public record CreateInterpreterResponse
{
    public Guid InterpreterId { get; init; }
    public Guid AgencyId { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public List<string> Skills { get; init; } = new();
    public DateTime CreatedAt { get; init; }
}
