using ThatInterpretingAgency.Core.Application.Common;

namespace ThatInterpretingAgency.Core.Application.Common;

public class AgencyUniquenessService : IAgencyUniquenessService
{
    private readonly IAgencyRepository _agencyRepository;
    
    public AgencyUniquenessService(IAgencyRepository agencyRepository)
    {
        _agencyRepository = agencyRepository;
    }
    
    public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeAgencyId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return true;
            
        var existingAgency = await _agencyRepository.GetByNameAsync(name, cancellationToken);
        
        // If no agency exists with this name, it's unique
        if (existingAgency == null)
            return true;
            
        // If we're excluding a specific agency (for updates), check if it's the same one
        if (excludeAgencyId.HasValue && existingAgency.Id == excludeAgencyId.Value)
            return true;
            
        // Name is not unique
        return false;
    }
}
