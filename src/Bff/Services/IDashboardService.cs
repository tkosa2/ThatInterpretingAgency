using Bff.DTOs;

namespace Bff.Services;

public interface IDashboardService
{
    Task<DashboardDTO> GetDashboardDataAsync();
    Task<DashboardDTO> GetAgencyDashboardAsync(Guid agencyId);
    Task<List<RecentAppointmentDTO>> GetRecentAppointmentsAsync(int count = 10);
    Task<List<UpcomingAppointmentDTO>> GetUpcomingAppointmentsAsync(int count = 10);
    Task<List<InterpreterRequestDTO>> GetRecentRequestsAsync(int count = 10);
}
