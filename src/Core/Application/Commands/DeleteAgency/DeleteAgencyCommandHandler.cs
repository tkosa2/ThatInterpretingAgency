using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.Domain.Aggregates;

namespace ThatInterpretingAgency.Core.Application.Commands.DeleteAgency;

public class DeleteAgencyCommandHandler : IRequestHandler<DeleteAgencyCommand, bool>
{
    private readonly IAgencyRepository _agencyRepository;

    public DeleteAgencyCommandHandler(IAgencyRepository agencyRepository)
    {
        _agencyRepository = agencyRepository;
    }

    public async Task<bool> Handle(DeleteAgencyCommand request, CancellationToken cancellationToken)
    {
        var agency = await _agencyRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (agency == null)
            return false;

        await _agencyRepository.DeleteAsync(agency, cancellationToken);
        return true;
    }
}
