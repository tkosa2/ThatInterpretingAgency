using Bff.DTOs;

namespace Bff.Services;

public interface IAgencyService
{
    Task<List<AgencySummaryDTO>> GetAgenciesAsync();
    Task<AgencyDetailDTO> GetAgencyDetailsAsync(Guid agencyId);
    Task<AgencySummaryDTO> CreateAgencyAsync(string name, string description);
    Task<bool> UpdateAgencyAsync(Guid agencyId, string name, string description);
    Task<bool> DeleteAgencyAsync(Guid agencyId);
    Task<List<AgencyStaffDTO>> GetAgencyStaffAsync(Guid agencyId);
    Task<bool> AddStaffMemberAsync(Guid agencyId, string userId, string role);
    Task<bool> RemoveStaffMemberAsync(Guid agencyId, string userId);
}
