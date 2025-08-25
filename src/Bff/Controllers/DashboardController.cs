using Bff.DTOs;
using Bff.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bff.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardDTO>> GetDashboard()
    {
        try
        {
            var dashboard = await _dashboardService.GetDashboardDataAsync();
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve dashboard data", details = ex.Message });
        }
    }

    [HttpGet("agency/{agencyId}")]
    public async Task<ActionResult<DashboardDTO>> GetAgencyDashboard(Guid agencyId)
    {
        try
        {
            var dashboard = await _dashboardService.GetAgencyDashboardAsync(agencyId);
            return Ok(dashboard);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve agency dashboard data", details = ex.Message });
        }
    }

    [HttpGet("recent-appointments")]
    public async Task<ActionResult<List<RecentAppointmentDTO>>> GetRecentAppointments([FromQuery] int count = 10)
    {
        try
        {
            var appointments = await _dashboardService.GetRecentAppointmentsAsync(count);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve recent appointments", details = ex.Message });
        }
    }

    [HttpGet("upcoming-appointments")]
    public async Task<ActionResult<List<UpcomingAppointmentDTO>>> GetUpcomingAppointments([FromQuery] int count = 10)
    {
        try
        {
            var appointments = await _dashboardService.GetUpcomingAppointmentsAsync(count);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve upcoming appointments", details = ex.Message });
        }
    }

    [HttpGet("recent-requests")]
    public async Task<ActionResult<List<InterpreterRequestDTO>>> GetRecentRequests([FromQuery] int count = 10)
    {
        try
        {
            var requests = await _dashboardService.GetRecentRequestsAsync(count);
            return Ok(requests);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve recent requests", details = ex.Message });
        }
    }
}
