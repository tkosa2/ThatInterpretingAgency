using MediatR;

namespace ThatInterpretingAgency.Core.Application.Commands.DeleteAgency;

public record DeleteAgencyCommand : IRequest<bool>
{
    public Guid Id { get; init; }
}
