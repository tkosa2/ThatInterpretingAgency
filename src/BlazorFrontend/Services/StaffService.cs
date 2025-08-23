using ThatInterpretingAgency.Core.DTOs;

namespace BlazorFrontend.Services;

public class StaffService : IStaffService
{
    private readonly IApiService _apiService;
    private readonly ILogger<StaffService> _logger;

    public StaffService(IApiService apiService, ILogger<StaffService> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    public async Task<List<StaffData>> GetStaffAsync()
    {
        try
        {
            return await _apiService.GetListAsync<StaffData>("api/staff");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff");
            return new List<StaffData>();
        }
    }

    public async Task<StaffData?> GetStaffMemberAsync(string id)
    {
        try
        {
            return await _apiService.GetAsync<StaffData>($"api/staff/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff member {StaffId}", id);
            return null;
        }
    }

    public async Task<StaffData?> CreateStaffMemberAsync(CreateStaffRequest request)
    {
        try
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(request.FullName))
                throw new ArgumentException("Full name is required");
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ArgumentException("Email is required");
            if (string.IsNullOrWhiteSpace(request.Role))
                throw new ArgumentException("Role is required");

            return await _apiService.PostAsync<StaffData>("api/staff", request);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating staff member");
            throw; // Re-throw validation errors for UI handling
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff member");
            return null;
        }
    }

    public async Task<StaffData?> UpdateStaffMemberAsync(string id, UpdateStaffRequest request)
    {
        try
        {
            return await _apiService.PutAsync<StaffData>($"api/staff/{id}", request);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff member {StaffId}", id);
            return null;
        }
    }

    public async Task<bool> DeleteStaffMemberAsync(string id)
    {
        try
        {
            return await _apiService.DeleteAsync($"api/staff/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff member {StaffId}", id);
            return false;
        }
    }

    public async Task<bool> ActivateStaffMemberAsync(string id)
    {
        try
        {
            return await _apiService.PutAsync<bool>($"api/staff/{id}/activate", new { });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating staff member {StaffId}", id);
            return false;
        }
    }

    public async Task<bool> DeactivateStaffMemberAsync(string id)
    {
        try
        {
            return await _apiService.PutAsync<bool>($"api/staff/{id}/deactivate", new { });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating staff member {StaffId}", id);
            return false;
        }
    }
}
