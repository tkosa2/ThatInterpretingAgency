using Bff.DTOs;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Bff.Services;

public class DashboardService : IDashboardService
{
    private readonly IAgencyRepository _agencyRepository;
    private readonly ThatInterpretingAgencyDbContext _context;

    public DashboardService(IAgencyRepository agencyRepository, ThatInterpretingAgencyDbContext context)
    {
        _agencyRepository = agencyRepository;
        _context = context;
    }

    public async Task<DashboardDTO> GetDashboardDataAsync()
    {
        var dashboard = new DashboardDTO();

        try
        {
            Console.WriteLine("BFF Dashboard: Starting GetDashboardDataAsync...");
            
            // Get agencies using direct context query
            var agencies = await _context.Agencies.ToListAsync();
            Console.WriteLine($"BFF Dashboard: Got agencies result, count: {agencies?.Count ?? 0}");
            
            dashboard.TotalAgencies = agencies?.Count ?? 0;

            // For now, use placeholder values for other metrics
            // These would be populated by actual queries when implemented
            dashboard.TotalInterpreters = 0;
            dashboard.TotalClients = 0;
            dashboard.TotalAppointments = 0;
            dashboard.PendingInterpreterRequests = 0;
            dashboard.TotalRevenue = 0;

            // Get recent appointments (placeholder)
            dashboard.RecentAppointments = await GetRecentAppointmentsAsync(5);

            // Get upcoming appointments (placeholder)
            dashboard.UpcomingAppointments = await GetUpcomingAppointmentsAsync(5);

            // Get recent requests (placeholder)
            dashboard.RecentRequests = await GetRecentRequestsAsync(5);
            
            Console.WriteLine($"BFF Dashboard: Final dashboard TotalAgencies: {dashboard.TotalAgencies}");
        }
        catch (Exception ex)
        {
            // Log the exception in a real application
            Console.WriteLine($"BFF Dashboard: Error getting dashboard data: {ex.Message}");
            Console.WriteLine($"BFF Dashboard: Stack trace: {ex.StackTrace}");
        }

        return dashboard;
    }

    public async Task<DashboardDTO> GetAgencyDashboardAsync(Guid agencyId)
    {
        var dashboard = new DashboardDTO();

        try
        {
            Console.WriteLine($"BFF Dashboard: Getting agency dashboard for ID: {agencyId}");
            
            // Get agency-specific data using direct context query
            var agency = await _context.Agencies.FirstOrDefaultAsync(a => a.Id == agencyId);
            
            if (agency != null)
            {
                Console.WriteLine($"BFF Dashboard: Found agency: {agency.Name}");
                dashboard.TotalAgencies = 1;
                
                // For now, use placeholder values
                // These would be populated by actual queries when implemented
                dashboard.TotalInterpreters = 0;
                dashboard.TotalClients = 0;
                dashboard.TotalAppointments = 0;
                dashboard.TotalRevenue = 0;
                dashboard.PendingInterpreterRequests = 0;
            }
            else
            {
                Console.WriteLine("BFF Dashboard: Agency not found");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BFF Dashboard: Error getting agency dashboard data: {ex.Message}");
            Console.WriteLine($"BFF Dashboard: Stack trace: {ex.StackTrace}");
        }

        return dashboard;
    }

    public async Task<List<RecentAppointmentDTO>> GetRecentAppointmentsAsync(int count = 10)
    {
        // Placeholder implementation - would be replaced with actual query
        var recentAppointments = new List<RecentAppointmentDTO>();

        // Return sample data for now
        for (int i = 0; i < Math.Min(count, 5); i++)
        {
            recentAppointments.Add(new RecentAppointmentDTO
            {
                Id = Guid.NewGuid(),
                AgencyName = $"Agency {i + 1}",
                InterpreterName = $"Interpreter {i + 1}",
                ClientName = $"Client {i + 1}",
                StartTime = DateTime.UtcNow.AddDays(-i),
                EndTime = DateTime.UtcNow.AddDays(-i).AddHours(2),
                Status = "Completed",
                Amount = 150.00m + (i * 25.00m)
            });
        }

        return recentAppointments;
    }

    public async Task<List<UpcomingAppointmentDTO>> GetUpcomingAppointmentsAsync(int count = 10)
    {
        // Placeholder implementation - would be replaced with actual query
        var upcomingAppointments = new List<UpcomingAppointmentDTO>();

        // Return sample data for now
        for (int i = 0; i < Math.Min(count, 5); i++)
        {
            upcomingAppointments.Add(new UpcomingAppointmentDTO
            {
                Id = Guid.NewGuid(),
                AgencyName = $"Agency {i + 1}",
                InterpreterName = $"Interpreter {i + 1}",
                ClientName = $"Client {i + 1}",
                StartTime = DateTime.UtcNow.AddDays(i + 1),
                EndTime = DateTime.UtcNow.AddDays(i + 1).AddHours(2),
                Location = $"Location {i + 1}",
                Language = $"Language {i + 1}"
            });
        }

        return upcomingAppointments;
    }

    public async Task<List<InterpreterRequestDTO>> GetRecentRequestsAsync(int count = 10)
    {
        // Placeholder implementation - would be replaced with actual query
        var recentRequests = new List<InterpreterRequestDTO>();

        // Return sample data for now
        for (int i = 0; i < Math.Min(count, 5); i++)
        {
            recentRequests.Add(new InterpreterRequestDTO
            {
                Id = Guid.NewGuid(),
                AgencyName = $"Agency {i + 1}",
                ClientName = $"Client {i + 1}",
                RequestedDate = DateTime.UtcNow.AddDays(-i),
                Language = $"Language {i + 1}",
                Status = "Pending",
                Description = $"Request description {i + 1}"
            });
        }

        return recentRequests;
    }
}
