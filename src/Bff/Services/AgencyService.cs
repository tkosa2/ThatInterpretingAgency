using Bff.DTOs;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bff.Services;

public class AgencyService : IAgencyService
{
    private readonly IAgencyRepository _agencyRepository;
    private readonly ThatInterpretingAgencyDbContext _context;

    public AgencyService(IAgencyRepository agencyRepository, ThatInterpretingAgencyDbContext context)
    {
        _agencyRepository = agencyRepository;
        _context = context;
    }

    public async Task<List<AgencySummaryDTO>> GetAgenciesAsync()
    {
        var agencies = new List<AgencySummaryDTO>();

        try
        {
            Console.WriteLine("BFF: Starting GetAgenciesAsync...");
            
            // Use direct database query like the controller does
            var rawAgencies = await _context.Agencies.ToListAsync();
            Console.WriteLine($"BFF: Direct context query returned {rawAgencies.Count} agencies");
            
            foreach (var agency in rawAgencies)
            {
                Console.WriteLine($"BFF: Processing agency: {agency.Name} (ID: {agency.Id})");
                
                var summary = new AgencySummaryDTO
                {
                    Id = agency.Id,
                    Name = agency.Name,
                    Description = agency.Description ?? string.Empty,
                    CreatedAt = agency.CreatedAt
                };

                // For now, use placeholder values
                // These would be populated by actual queries when implemented
                summary.InterpreterCount = 0;
                summary.ClientCount = 0;
                summary.AppointmentCount = 0;
                summary.TotalRevenue = 0;

                agencies.Add(summary);
            }
            
            Console.WriteLine($"BFF: Final agencies list count: {agencies.Count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BFF: Error getting agencies: {ex.Message}");
            Console.WriteLine($"BFF: Stack trace: {ex.StackTrace}");
        }

        return agencies;
    }

    public async Task<AgencyDetailDTO> GetAgencyDetailsAsync(Guid agencyId)
    {
        try
        {
            Console.WriteLine($"BFF: Getting agency details for ID: {agencyId}");
            
            var agency = await _context.Agencies.FirstOrDefaultAsync(a => a.Id == agencyId);

            if (agency == null)
            {
                Console.WriteLine("BFF: Agency not found");
                return new AgencyDetailDTO();
            }

            Console.WriteLine($"BFF: Found agency: {agency.Name}");

            var detail = new AgencyDetailDTO
            {
                Id = agency.Id,
                Name = agency.Name,
                Description = agency.Description ?? string.Empty,
                CreatedAt = agency.CreatedAt,
                UpdatedAt = agency.UpdatedAt
            };

            // For now, use placeholder data
            // These would be populated by actual queries when implemented
            detail.Staff = new List<AgencyStaffDTO>();
            detail.Interpreters = new List<InterpreterSummaryDTO>();
            detail.Clients = new List<ClientSummaryDTO>();
            detail.RecentAppointments = new List<AppointmentSummaryDTO>();

            return detail;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BFF: Error getting agency details: {ex.Message}");
            Console.WriteLine($"BFF: Stack trace: {ex.StackTrace}");
            return new AgencyDetailDTO();
        }
    }

    public async Task<AgencySummaryDTO> CreateAgencyAsync(string name, string description)
    {
        // Placeholder implementation - would be replaced with actual command
        throw new NotImplementedException("CreateAgency command not yet implemented");
    }

    public async Task<bool> UpdateAgencyAsync(Guid agencyId, string name, string description)
    {
        // Placeholder implementation - would be replaced with actual command
        throw new NotImplementedException("UpdateAgency command not yet implemented");
    }

    public async Task<bool> DeleteAgencyAsync(Guid agencyId)
    {
        // Placeholder implementation - would be replaced with actual command
        return false;
    }

    public async Task<List<AgencyStaffDTO>> GetAgencyStaffAsync(Guid agencyId)
    {
        // Placeholder implementation - would be replaced with actual query
        return new List<AgencyStaffDTO>();
    }

    public async Task<bool> AddStaffMemberAsync(Guid agencyId, string userId, string role)
    {
        // Placeholder implementation - would be replaced with actual command
        return false;
    }

    public async Task<bool> RemoveStaffMemberAsync(Guid agencyId, string userId)
    {
        // Placeholder implementation - would be replaced with actual command
        return false;
    }
}
