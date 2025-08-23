using ThatInterpretingAgency.Core.DTOs;

namespace BlazorFrontend.Services;

public class AgencyService : IAgencyService
{
    private readonly IApiService _apiService;
    private readonly ILogger<AgencyService> _logger;

    public AgencyService(IApiService apiService, ILogger<AgencyService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<List<AgencyData>> GetAgenciesAsync()
    {
        try
        {
            return await _apiService.GetListAsync<AgencyData>("api/agencies");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting agencies");
            return new List<AgencyData>();
        }
    }

    public async Task<AgencyData?> GetAgencyAsync(string id)
    {
        try
        {
            return await _apiService.GetAsync<AgencyData>($"api/agencies/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting agency {AgencyId}", id);
            return null;
        }
    }

    public async Task<AgencyData?> CreateAgencyAsync(CreateAgencyRequest request)
    {
        try
        {
            return await _apiService.PostAsync<AgencyData>("api/agencies", request);
        }
        catch (ApiException ex) when (ex.Message.Contains("already exists"))
        {
            // Re-throw the specific duplicate name error for UI handling
            throw new InvalidOperationException("Agency already exists. Please use a different agency name.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating agency");
            return null;
        }
    }

    public async Task<AgencyData?> UpdateAgencyAsync(string id, UpdateAgencyRequest request)
    {
        try
        {
            return await _apiService.PutAsync<AgencyData>($"api/agencies/{id}", request);
        }
        catch (ApiException ex) when (ex.Message.Contains("already exists"))
        {
            // Re-throw the specific duplicate name error for UI handling
            throw new InvalidOperationException("Agency already exists. Please use a different agency name.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating agency {AgencyId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteAgencyAsync(string id)
    {
        try
        {
            return await _apiService.DeleteAsync($"api/agencies/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting agency {AgencyId}", id);
            return false;
        }
    }

    public async Task<AgencyStats?> GetAgencyStatsAsync(string agencyId)
    {
        try
        {
            return await _apiService.GetAsync<AgencyStats>($"api/agencies/{agencyId}/stats");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting agency stats for {AgencyId}", agencyId);
            return null;
        }
    }
}
