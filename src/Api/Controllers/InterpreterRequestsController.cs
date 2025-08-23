using Microsoft.AspNetCore.Mvc;
using ThatInterpretingAgency.Core.DTOs;
using ThatInterpretingAgency.Core.Application.Common;

namespace ThatInterpretingAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InterpreterRequestsController : ControllerBase
{
    private readonly ILogger<InterpreterRequestsController> _logger;
    private readonly IInterpreterRequestService _interpreterRequestService;

    public InterpreterRequestsController(
        ILogger<InterpreterRequestsController> logger,
        IInterpreterRequestService interpreterRequestService)
    {
        _logger = logger;
        _interpreterRequestService = interpreterRequestService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<InterpreterRequestData>>> GetInterpreterRequests(
        [FromQuery] string? agencyId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? language = null)
    {
        try
        {
            // Try to use real database service first
            try
            {
                var requests = await _interpreterRequestService.GetInterpreterRequestsAsync(
                    agencyId: !string.IsNullOrEmpty(agencyId) ? Guid.Parse(agencyId) : null,
                    status: status,
                    language: language);
                
                return Ok(requests);
            }
            catch (Exception dbEx)
            {
                _logger.LogWarning(dbEx, "Database service failed, falling back to mock data");
                
                // Fallback to mock data for testing
                var mockRequests = new List<InterpreterRequestData>
                {
                    new InterpreterRequestData
                    {
                        Id = "1",
                        AgencyId = "1",
                        RequestorId = "1",
                        AppointmentType = "Virtual",
                        VirtualMeetingLink = "https://meet.google.com/abc-defg-hij",
                        Location = "Virtual Meeting",
                        Mode = "Consecutive",
                        Description = "Medical consultation with Spanish-speaking patient",
                        RequestedDate = DateTime.Today.AddDays(7),
                        StartTime = DateTime.Today.AddDays(7).AddHours(9),
                        EndTime = DateTime.Today.AddDays(7).AddHours(11),
                        Language = "Spanish",
                        SpecialInstructions = "Medical terminology expertise required",
                        Status = "Pending",
                        Division = "Healthcare",
                        Program = "Patient Care",
                        RequestorName = "Dr. Sarah Johnson",
                        OrganizationName = "City General Hospital",
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    },
                    new InterpreterRequestData
                    {
                        Id = "2",
                        AgencyId = "1",
                        RequestorId = "2",
                        AppointmentType = "In-Person",
                        Location = "123 Main Street, Court Building",
                        Mode = "Simultaneous",
                        Description = "Legal deposition for French-speaking witness",
                        RequestedDate = DateTime.Today.AddDays(3),
                        StartTime = DateTime.Today.AddDays(3).AddHours(14),
                        EndTime = DateTime.Today.AddDays(3).AddHours(16),
                        Language = "French",
                        SpecialInstructions = "Legal terminology expertise required",
                        Status = "Approved",
                        Division = "Legal Services",
                        Program = "Court Proceedings",
                        RequestorName = "Attorney Mike Davis",
                        OrganizationName = "Davis & Associates Law Firm",
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    }
                };

                // Apply filters
                if (!string.IsNullOrEmpty(agencyId))
                    mockRequests = mockRequests.Where(r => r.AgencyId == agencyId).ToList();
                
                if (!string.IsNullOrEmpty(status))
                    mockRequests = mockRequests.Where(r => r.Status == status).ToList();
                
                if (!string.IsNullOrEmpty(language))
                    mockRequests = mockRequests.Where(r => r.Language == language).ToList();

                return Ok(mockRequests);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting interpreter requests");
            return StatusCode(500, new { error = "An error occurred while retrieving interpreter requests" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InterpreterRequestData>> GetInterpreterRequest(string id)
    {
        try
        {
            // TODO: Implement actual database query
            var mockRequest = new InterpreterRequestData
            {
                Id = id,
                AgencyId = "1",
                RequestorId = "1",
                AppointmentType = "Virtual",
                VirtualMeetingLink = "https://meet.google.com/abc-defg-hij",
                Location = "Virtual Meeting",
                Mode = "Consecutive",
                Description = "Medical consultation with Spanish-speaking patient",
                RequestedDate = DateTime.Today.AddDays(7),
                StartTime = DateTime.Today.AddDays(7).AddHours(9),
                EndTime = DateTime.Today.AddDays(7).AddHours(11),
                Language = "Spanish",
                SpecialInstructions = "Medical terminology expertise required",
                Status = "Pending",
                Division = "Healthcare",
                Program = "Patient Care",
                RequestorName = "Dr. Sarah Johnson",
                OrganizationName = "City General Hospital",
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };

            return Ok(mockRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting interpreter request {RequestId}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the interpreter request" });
        }
    }

    [HttpGet("client/{clientId}")]
    public async Task<ActionResult<IEnumerable<InterpreterRequestData>>> GetClientRequests(string clientId)
    {
        try
        {
            // TODO: Implement actual database query for client-specific requests
            var mockRequests = new List<InterpreterRequestData>
            {
                new InterpreterRequestData
                {
                    Id = "1",
                    AgencyId = "1",
                    RequestorId = clientId,
                    AppointmentType = "Virtual",
                    VirtualMeetingLink = "https://meet.google.com/abc-defg-hij",
                    Location = "Virtual Meeting",
                    Mode = "Consecutive",
                    Description = "Medical consultation with Spanish-speaking patient",
                    RequestedDate = DateTime.Today.AddDays(7),
                    StartTime = DateTime.Today.AddDays(7).AddHours(9),
                    EndTime = DateTime.Today.AddDays(7).AddHours(11),
                    Language = "Spanish",
                    SpecialInstructions = "Medical terminology expertise required",
                    Status = "Pending",
                    Division = "Healthcare",
                    Program = "Patient Care",
                    RequestorName = "Dr. Sarah Johnson",
                    OrganizationName = "City General Hospital",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };

            return Ok(mockRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting client requests for {ClientId}", clientId);
            return StatusCode(500, new { error = "An error occurred while retrieving client requests" });
        }
    }

    [HttpGet("approved")]
    public async Task<ActionResult<IEnumerable<InterpreterRequestData>>> GetApprovedRequests()
    {
        try
        {
            // TODO: Implement actual database query for approved requests
            var mockRequests = new List<InterpreterRequestData>
            {
                new InterpreterRequestData
                {
                    Id = "2",
                    AgencyId = "1",
                    RequestorId = "2",
                    AppointmentType = "In-Person",
                    Location = "123 Main Street, Court Building",
                    Mode = "Simultaneous",
                    Description = "Legal deposition for French-speaking witness",
                    RequestedDate = DateTime.Today.AddDays(3),
                    StartTime = DateTime.Today.AddDays(3).AddHours(14),
                    EndTime = DateTime.Today.AddDays(3).AddHours(16),
                    Language = "French",
                    SpecialInstructions = "Legal terminology expertise required",
                    Status = "Approved",
                    Division = "Legal Services",
                    Program = "Court Proceedings",
                    RequestorName = "Attorney Mike Davis",
                    OrganizationName = "Davis & Associates Law Firm",
                    CreatedAt = DateTime.UtcNow.AddDays(-3)
                }
            };

            return Ok(mockRequests);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting approved interpreter requests");
            return StatusCode(500, new { error = "An error occurred while retrieving approved requests" });
        }
    }

    [HttpPost]
    public async Task<ActionResult<InterpreterRequestData>> CreateInterpreterRequest([FromBody] CreateInterpreterRequestRequest request)
    {
        try
        {
            // Try to use real database service first
            try
            {
                var createdRequest = await _interpreterRequestService.CreateInterpreterRequestAsync(request);
                return CreatedAtAction(nameof(GetInterpreterRequest), new { id = createdRequest.Id }, createdRequest);
            }
            catch (Exception dbEx)
            {
                _logger.LogWarning(dbEx, "Database service failed, falling back to mock data");
                
                // Fallback to mock data for testing
                var createdRequest = new InterpreterRequestData
                {
                    Id = Guid.NewGuid().ToString(),
                    AgencyId = request.AgencyId,
                    RequestorId = request.RequestorId,
                    AppointmentType = request.AppointmentType,
                    VirtualMeetingLink = request.VirtualMeetingLink,
                    Location = request.Location,
                    Mode = request.Mode,
                    Description = request.Description,
                    RequestedDate = request.RequestedDate,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Language = request.Language,
                    SpecialInstructions = request.SpecialInstructions,
                    Status = "Pending",
                    Division = request.Division,
                    Program = request.Program,
                    LniContact = request.LniContact,
                    DayOfEventContact = request.DayOfEventContact,
                    DayOfEventContactPhone = request.DayOfEventContactPhone,
                    CostCode = request.CostCode,
                    InvoiceApprover = request.InvoiceApprover,
                    SupportingMaterials = request.SupportingMaterials,
                    CreatedAt = DateTime.UtcNow
                };

                return CreatedAtAction(nameof(GetInterpreterRequest), new { id = createdRequest.Id }, createdRequest);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating interpreter request");
            return StatusCode(500, new { error = "An error occurred while creating the interpreter request" });
        }
    }

    [HttpPut("{id}/status")]
    public async Task<ActionResult<InterpreterRequestData>> UpdateRequestStatus(string id, [FromBody] UpdateInterpreterRequestStatusRequest request)
    {
        try
        {
            // Try to use real database service first
            try
            {
                var updatedRequest = await _interpreterRequestService.UpdateRequestStatusAsync(Guid.Parse(id), request);
                return Ok(updatedRequest);
            }
            catch (Exception dbEx)
            {
                _logger.LogWarning(dbEx, "Database service failed, falling back to mock data");
                
                // Fallback to mock data for testing
                var updatedRequest = new InterpreterRequestData
                {
                    Id = id,
                    AgencyId = "1",
                    RequestorId = "1",
                    AppointmentType = "Virtual",
                    VirtualMeetingLink = "https://meet.google.com/abc-defg-hij",
                    Location = "Virtual Meeting",
                    Mode = "Consecutive",
                    Description = "Medical consultation with Spanish-speaking patient",
                    RequestedDate = DateTime.Today.AddDays(7),
                    StartTime = DateTime.Today.AddDays(7).AddHours(9),
                    EndTime = DateTime.Today.AddDays(7).AddHours(11),
                    Language = "Spanish",
                    SpecialInstructions = "Medical terminology expertise required",
                    Status = request.Status,
                    Division = "Healthcare",
                    Program = "Patient Care",
                    RequestorName = "Dr. Sarah Johnson",
                    OrganizationName = "City General Hospital",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                };

                return Ok(updatedRequest);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating interpreter request status {RequestId}", id);
            return StatusCode(500, new { error = "An error occurred while updating the request status" });
        }
    }

    [HttpPut("{id}/cancel")]
    public async Task<ActionResult<InterpreterRequestData>> CancelRequest(string id)
    {
        try
        {
            // Try to use real database service first
            try
            {
                var cancelledRequest = await _interpreterRequestService.CancelRequestAsync(Guid.Parse(id));
                return Ok(cancelledRequest);
            }
            catch (Exception dbEx)
            {
                _logger.LogWarning(dbEx, "Database service failed, falling back to mock data");
                
                // Fallback to mock data for testing
                var cancelledRequest = new InterpreterRequestData
                {
                    Id = id,
                    AgencyId = "1",
                    RequestorId = "1",
                    AppointmentType = "Virtual",
                    VirtualMeetingLink = "https://meet.google.com/abc-defg-hij",
                    Location = "Virtual Meeting",
                    Mode = "Consecutive",
                    Description = "Medical consultation with Spanish-speaking patient",
                    RequestedDate = DateTime.Today.AddDays(7),
                    StartTime = DateTime.Today.AddDays(7).AddHours(9),
                    EndTime = DateTime.Today.AddDays(7).AddHours(11),
                    Language = "Spanish",
                    SpecialInstructions = "Medical terminology expertise required",
                    Status = "Cancelled",
                    Division = "Healthcare",
                    Program = "Patient Care",
                    RequestorName = "Dr. Sarah Johnson",
                    OrganizationName = "City General Hospital",
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                };

                return Ok(cancelledRequest);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling interpreter request {RequestId}", id);
            return StatusCode(500, new { error = "An error occurred while cancelling the request" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteInterpreterRequest(string id)
    {
        try
        {
            // Try to use real database service first
            try
            {
                var deleted = await _interpreterRequestService.DeleteInterpreterRequestAsync(Guid.Parse(id));
                if (deleted)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception dbEx)
            {
                _logger.LogWarning(dbEx, "Database service failed, falling back to mock data");
                
                // Fallback to mock data for testing - simulate successful deletion
                return NoContent();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting interpreter request {RequestId}", id);
            return StatusCode(500, new { error = "An error occurred while deleting the interpreter request" });
        }
    }
}
