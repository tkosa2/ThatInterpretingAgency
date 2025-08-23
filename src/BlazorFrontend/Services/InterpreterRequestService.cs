using ThatInterpretingAgency.Core.DTOs;

namespace BlazorFrontend.Services;

public class InterpreterRequestService : IInterpreterRequestService
{
    private readonly IApiService _apiService;
    private readonly ILogger<InterpreterRequestService> _logger;

    public InterpreterRequestService(IApiService apiService, ILogger<InterpreterRequestService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<List<InterpreterRequestData>> GetInterpreterRequestsAsync(string? agencyId = null, string? status = null, string? language = null)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(agencyId)) queryParams.Add($"agencyId={agencyId}");
            if (!string.IsNullOrEmpty(status)) queryParams.Add($"status={status}");
            if (!string.IsNullOrEmpty(language)) queryParams.Add($"language={language}");

            var endpoint = "api/interpreterrequests";
            if (queryParams.Any())
                endpoint += "?" + string.Join("&", queryParams);

            return await _apiService.GetListAsync<InterpreterRequestData>(endpoint);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting interpreter requests");
            return new List<InterpreterRequestData>();
        }
    }

    public async Task<InterpreterRequestData?> GetInterpreterRequestAsync(string id)
    {
        try
        {
            return await _apiService.GetAsync<InterpreterRequestData>($"api/interpreterrequests/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting interpreter request {RequestId}", id);
            return null;
        }
    }

    public async Task<List<InterpreterRequestData>> GetClientRequestsAsync(string clientId)
    {
        try
        {
            return await _apiService.GetListAsync<InterpreterRequestData>($"api/interpreterrequests/client/{clientId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client requests for {ClientId}", clientId);
            return new List<InterpreterRequestData>();
        }
    }

    public async Task<List<InterpreterRequestData>> GetApprovedRequestsAsync()
    {
        try
        {
            return await _apiService.GetListAsync<InterpreterRequestData>("api/interpreterrequests/approved");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting approved interpreter requests");
            return new List<InterpreterRequestData>();
        }
    }

    public async Task<InterpreterRequestData?> CreateInterpreterRequestAsync(CreateInterpreterRequestRequest request)
    {
        try
        {
            return await _apiService.PostAsync<InterpreterRequestData>("api/interpreterrequests", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating interpreter request");
            return null;
        }
    }

    public async Task<InterpreterRequestData?> UpdateRequestStatusAsync(string id, UpdateInterpreterRequestStatusRequest request)
    {
        try
        {
            return await _apiService.PutAsync<InterpreterRequestData>($"api/interpreterrequests/{id}/status", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating interpreter request status {RequestId}", id);
            return null;
        }
    }

    public async Task<InterpreterRequestData?> CancelRequestAsync(string id)
    {
        try
        {
            return await _apiService.PutAsync<InterpreterRequestData>($"api/interpreterrequests/{id}/cancel", new { });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling interpreter request {RequestId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteInterpreterRequestAsync(string id)
    {
        try
        {
            return await _apiService.DeleteAsync($"api/interpreterrequests/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting interpreter request {RequestId}", id);
            return false;
        }
    }
}

