namespace ThatInterpretingAgency.Core.Application.Common;

public interface IAgencyUniquenessService
{
    Task<bool> IsNameUniqueAsync(string name, Guid? excludeAgencyId = null, CancellationToken cancellationToken = default);
}
