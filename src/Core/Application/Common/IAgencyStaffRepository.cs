using ThatInterpretingAgency.Core.Domain.Entities;

namespace ThatInterpretingAgency.Core.Application.Common;

public interface IAgencyStaffRepository
{
    Task<AgencyStaff?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AgencyStaff>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<AgencyStaff>> GetByAgencyIdAsync(Guid agencyId, CancellationToken cancellationToken = default);
    Task<AgencyStaff?> GetByAgencyAndUserIdAsync(Guid agencyId, Guid userId, CancellationToken cancellationToken = default);
    Task<AgencyStaff> AddAsync(AgencyStaff entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(AgencyStaff entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(AgencyStaff entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UserHasRoleInAgencyAsync(Guid userId, Guid agencyId, string role, CancellationToken cancellationToken = default);
}
