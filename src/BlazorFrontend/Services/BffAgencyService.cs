using BlazorFrontend.Data;
using ThatInterpretingAgency.Core.DTOs;

namespace BlazorFrontend.Services;

public class BffAgencyService : IAgencyService
{
    private readonly IBffService _bffService;
    private readonly ILogger<BffAgencyService> _logger;

    public BffAgencyService(IBffService bffService, ILogger<BffAgencyService> logger)
    {
        _bffService = bffService;
        _logger = logger;
    }

    public async Task<List<ThatInterpretingAgency.Core.DTOs.AgencyData>> GetAgenciesAsync()
    {
        try
        {
            // Call BFF endpoint
            var bffAgencies = await _bffService.GetListAsync<BffAgencySummaryDTO>("api/agency");
            
            // Convert BFF DTOs to Core DTOs for backward compatibility
            var agencies = new List<ThatInterpretingAgency.Core.DTOs.AgencyData>();
            foreach (var bffAgency in bffAgencies)
            {
                agencies.Add(new ThatInterpretingAgency.Core.DTOs.AgencyData
                {
                    Id = bffAgency.Id.ToString(),
                    Name = bffAgency.Name,
                    ContactInfo = bffAgency.Description,
                    Address = string.Empty, // Not available in BFF yet
                    Phone = string.Empty,   // Not available in BFF yet
                    Email = string.Empty,   // Not available in BFF yet
                    Status = "Active",      // Default status
                    CreatedAt = bffAgency.CreatedAt,
                    UpdatedAt = bffAgency.CreatedAt // Use CreatedAt as fallback
                });
            }
            
            return agencies;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting agencies from BFF");
            return new List<ThatInterpretingAgency.Core.DTOs.AgencyData>();
        }
    }

    public async Task<ThatInterpretingAgency.Core.DTOs.AgencyData?> GetAgencyAsync(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var agencyId))
            {
                _logger.LogWarning("Invalid agency ID format: {AgencyId}", id);
                return null;
            }

            // Call BFF endpoint
            var bffAgency = await _bffService.GetAsync<BffAgencyDetailDTO>($"api/agency/{agencyId}");
            
            if (bffAgency == null)
                return null;

            // Convert BFF DTO to Core DTO
            return new ThatInterpretingAgency.Core.DTOs.AgencyData
            {
                Id = bffAgency.Id.ToString(),
                Name = bffAgency.Name,
                ContactInfo = bffAgency.Description,
                Address = string.Empty, // Not available in BFF yet
                Phone = string.Empty,   // Not available in BFF yet
                Email = string.Empty,   // Not available in BFF yet
                Status = "Active",      // Default status
                CreatedAt = bffAgency.CreatedAt,
                UpdatedAt = bffAgency.UpdatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting agency {AgencyId} from BFF", id);
            return null;
        }
    }

    public async Task<ThatInterpretingAgency.Core.DTOs.AgencyData?> CreateAgencyAsync(ThatInterpretingAgency.Core.DTOs.CreateAgencyRequest request)
    {
        try
        {
            // Call BFF endpoint
            var bffRequest = new { name = request.Name, description = request.ContactInfo };
            var bffAgency = await _bffService.PostAsync<BffAgencySummaryDTO>("api/agency", bffRequest);
            
            if (bffAgency == null)
                return null;

            // Convert BFF DTO to Core DTO
            return new ThatInterpretingAgency.Core.DTOs.AgencyData
            {
                Id = bffAgency.Id.ToString(),
                Name = bffAgency.Name,
                ContactInfo = bffAgency.Description,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                Status = "Active",
                CreatedAt = bffAgency.CreatedAt,
                UpdatedAt = bffAgency.CreatedAt
            };
        }
        catch (ApiException ex) when (ex.Message.Contains("already exists"))
        {
            throw new InvalidOperationException("Agency already exists. Please use a different agency name.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating agency via BFF");
            return null;
        }
    }

    public async Task<ThatInterpretingAgency.Core.DTOs.AgencyData?> UpdateAgencyAsync(string id, ThatInterpretingAgency.Core.DTOs.UpdateAgencyRequest request)
    {
        try
        {
            if (!Guid.TryParse(id, out var agencyId))
            {
                _logger.LogWarning("Invalid agency ID format: {AgencyId}", id);
                return null;
            }

            // Call BFF endpoint
            var bffRequest = new { name = request.Name, description = request.ContactInfo };
            var bffAgency = await _bffService.PutAsync<BffAgencySummaryDTO>($"api/agency/{agencyId}", bffRequest);
            
            if (bffAgency == null)
                return null;

            // Convert BFF DTO to Core DTO
            return new ThatInterpretingAgency.Core.DTOs.AgencyData
            {
                Id = bffAgency.Id.ToString(),
                Name = bffAgency.Name,
                ContactInfo = bffAgency.Description,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                Status = request.Status,
                CreatedAt = bffAgency.CreatedAt,
                UpdatedAt = bffAgency.CreatedAt
            };
        }
        catch (ApiException ex) when (ex.Message.Contains("already exists"))
        {
            throw new InvalidOperationException("Agency already exists. Please use a different agency name.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating agency {AgencyId} via BFF", id);
            return null;
        }
    }

    public async Task<bool> DeleteAgencyAsync(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var agencyId))
            {
                _logger.LogWarning("Invalid agency ID format: {AgencyId}", id);
                return false;
            }

            // Call BFF endpoint
            return await _bffService.DeleteAsync($"api/agency/{agencyId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting agency {AgencyId} via BFF", id);
            return false;
        }
    }

    public async Task<ThatInterpretingAgency.Core.DTOs.AgencyStats?> GetAgencyStatsAsync(string agencyId)
    {
        try
        {
            if (!Guid.TryParse(agencyId, out var id))
            {
                _logger.LogWarning("Invalid agency ID format: {AgencyId}", agencyId);
                return null;
            }

            // Call BFF dashboard endpoint for agency-specific stats
            var dashboard = await _bffService.GetAsync<BffDashboardDTO>($"api/dashboard/agency/{id}");
            
            if (dashboard == null)
                return null;

            // Convert BFF DTO to Core DTO
            return new ThatInterpretingAgency.Core.DTOs.AgencyStats
            {
                TotalStaff = dashboard.TotalInterpreters + dashboard.TotalClients, // Approximate
                ActiveInterpreters = dashboard.TotalInterpreters,
                TotalClients = dashboard.TotalClients,
                TotalAppointments = dashboard.TotalAppointments,
                MonthlyRevenue = dashboard.TotalRevenue, // Assuming this is monthly
                PendingInvoices = dashboard.PendingInterpreterRequests
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting agency stats for {AgencyId} via BFF", agencyId);
            return null;
        }
    }
}
