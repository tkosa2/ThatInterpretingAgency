using Bff.DTOs;
using Bff.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThatInterpretingAgency.Infrastructure.Persistence;

namespace Bff.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgencyController : ControllerBase
{
    private readonly IAgencyService _agencyService;
    private readonly ThatInterpretingAgencyDbContext _context;

    public AgencyController(IAgencyService agencyService, ThatInterpretingAgencyDbContext context)
    {
        _agencyService = agencyService;
        _context = context;
    }

    [HttpGet("test-db")]
    public async Task<ActionResult> TestDatabase()
    {
        try
        {
            Console.WriteLine("BFF Controller: Testing database connection...");
            
            // Direct database query to test connection
            var agencies = await _context.Agencies.ToListAsync();
            Console.WriteLine($"BFF Controller: Direct DB query returned {agencies.Count} agencies");
            
            return Ok(new { 
                connection = "success", 
                agencyCount = agencies.Count,
                agencies = agencies.Select(a => new { a.Id, a.Name, a.Description, a.CreatedAt })
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BFF Controller: Database test failed: {ex.Message}");
            return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }

    [HttpGet]
    public async Task<ActionResult<List<AgencySummaryDTO>>> GetAgencies()
    {
        try
        {
            Console.WriteLine("BFF Controller: GetAgencies called");
            var agencies = await _agencyService.GetAgenciesAsync();
            Console.WriteLine($"BFF Controller: GetAgencies returned {agencies.Count} agencies");
            return Ok(agencies);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BFF Controller: GetAgencies failed: {ex.Message}");
            return StatusCode(500, new { error = "Failed to retrieve agencies", details = ex.Message });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AgencyDetailDTO>> GetAgency(Guid id)
    {
        try
        {
            var agency = await _agencyService.GetAgencyDetailsAsync(id);
            
            if (agency.Id == Guid.Empty)
                return NotFound(new { error = "Agency not found" });

            return Ok(agency);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve agency details", details = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<AgencySummaryDTO>> CreateAgency([FromBody] CreateAgencyRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { error = "Agency name is required" });

            var agency = await _agencyService.CreateAgencyAsync(request.Name, request.Description ?? string.Empty);
            return CreatedAtAction(nameof(GetAgency), new { id = agency.Id }, agency);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to create agency", details = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateAgency(Guid id, [FromBody] UpdateAgencyRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return BadRequest(new { error = "Agency name is required" });

            var success = await _agencyService.UpdateAgencyAsync(id, request.Name, request.Description ?? string.Empty);
            
            if (!success)
                return NotFound(new { error = "Agency not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to update agency", details = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAgency(Guid id)
    {
        try
        {
            var success = await _agencyService.DeleteAgencyAsync(id);
            
            if (!success)
                return NotFound(new { error = "Agency not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to delete agency", details = ex.Message });
        }
    }

    [HttpGet("{id}/staff")]
    public async Task<ActionResult<List<AgencyStaffDTO>>> GetAgencyStaff(Guid id)
    {
        try
        {
            var staff = await _agencyService.GetAgencyStaffAsync(id);
            return Ok(staff);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to retrieve agency staff", details = ex.Message });
        }
    }

    [HttpPost("{id}/staff")]
    public async Task<ActionResult> AddStaffMember(Guid id, [FromBody] AddStaffMemberRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Role))
                return BadRequest(new { error = "UserId and Role are required" });

            var success = await _agencyService.AddStaffMemberAsync(id, request.UserId, request.Role);
            
            if (!success)
                return NotFound(new { error = "Agency not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to add staff member", details = ex.Message });
        }
    }

    [HttpDelete("{id}/staff/{userId}")]
    public async Task<ActionResult> RemoveStaffMember(Guid id, string userId)
    {
        try
        {
            var success = await _agencyService.RemoveStaffMemberAsync(id, userId);
            
            if (!success)
                return NotFound(new { error = "Agency or staff member not found" });

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "Failed to remove staff member", details = ex.Message });
        }
    }
}

public class CreateAgencyRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class UpdateAgencyRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class AddStaffMemberRequest
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
