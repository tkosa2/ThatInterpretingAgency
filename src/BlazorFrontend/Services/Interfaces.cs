using ThatInterpretingAgency.Core.DTOs;

namespace BlazorFrontend.Services;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task<bool> DeleteAsync(string endpoint);
    Task<List<T>> GetListAsync<T>(string endpoint);
}

public interface IBffService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task<bool> DeleteAsync(string endpoint);
    Task<List<T>> GetListAsync<T>(string endpoint);
}

public interface IAgencyService
{
    Task<List<AgencyData>> GetAgenciesAsync();
    Task<AgencyData?> GetAgencyAsync(string id);
    Task<AgencyData?> CreateAgencyAsync(CreateAgencyRequest request);
    Task<AgencyData?> UpdateAgencyAsync(string id, UpdateAgencyRequest request);
    Task<bool> DeleteAgencyAsync(string id);
    Task<AgencyStats?> GetAgencyStatsAsync(string agencyId);
}

public interface IStaffService
{
    Task<List<StaffData>> GetStaffAsync();
    Task<StaffData?> GetStaffMemberAsync(string id);
    Task<StaffData?> CreateStaffMemberAsync(CreateStaffRequest request);
    Task<StaffData?> UpdateStaffMemberAsync(string id, UpdateStaffRequest request);
    Task<bool> DeleteStaffMemberAsync(string id);
    Task<bool> ActivateStaffMemberAsync(string id);
    Task<bool> DeactivateStaffMemberAsync(string id);
}

public interface IInterpreterRequestService
{
    Task<List<InterpreterRequestData>> GetInterpreterRequestsAsync(string? agencyId = null, string? status = null, string? language = null);
    Task<InterpreterRequestData?> GetInterpreterRequestAsync(string id);
    Task<List<InterpreterRequestData>> GetClientRequestsAsync(string clientId);
    Task<List<InterpreterRequestData>> GetApprovedRequestsAsync();
    Task<InterpreterRequestData?> CreateInterpreterRequestAsync(CreateInterpreterRequestRequest request);
    Task<InterpreterRequestData?> UpdateRequestStatusAsync(string id, UpdateInterpreterRequestStatusRequest request);
    Task<InterpreterRequestData?> CancelRequestAsync(string id);
    Task<bool> DeleteInterpreterRequestAsync(string id);
}
