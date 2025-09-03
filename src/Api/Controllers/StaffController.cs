using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ThatInterpretingAgency.Core.DTOs;
using ThatInterpretingAgency.Core.Application.Common;
using ThatInterpretingAgency.Core.Domain.Entities;

namespace ThatInterpretingAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StaffController : ControllerBase
{
    private readonly ILogger<StaffController> _logger;
    private readonly IAgencyStaffRepository _agencyStaffRepository;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IAgencyRepository _agencyRepository;

    public StaffController(ILogger<StaffController> logger, IAgencyStaffRepository agencyStaffRepository, UserManager<IdentityUser> userManager, IAgencyRepository agencyRepository)
    {
        _logger = logger;
        _agencyStaffRepository = agencyStaffRepository;
        _userManager = userManager;
        _agencyRepository = agencyRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<StaffData>>> GetStaff()
    {
        try
        {
            // Get all agency staff records
            var agencyStaff = await _agencyStaffRepository.GetAllAsync();
            var staffData = new List<StaffData>();

            foreach (var staff in agencyStaff)
            {
                // Get user details from AspNetUsers
                var user = await _userManager.FindByIdAsync(staff.UserId.ToString());
                if (user != null)
                {
                    var staffDataItem = new StaffData
                    {
                        Id = staff.Id.ToString(),
                        FullName = user.UserName ?? user.Email, // Use UserName or fallback to Email
                        Role = staff.Role,
                        Email = user.Email,
                        Phone = user.PhoneNumber ?? "",
                        HireDate = staff.HireDate,
                        HourlyRate = 0, // TODO: Add hourly rate to AgencyStaff or separate table
                        Status = staff.Status.ToString(),
                        Address = "", // TODO: Add address to user profile or separate table
                        Notes = "", // TODO: Add notes to AgencyStaff or separate table
                        Languages = new List<string>(), // TODO: Add languages to AgencyStaff or separate table
                        Specializations = "" // TODO: Add specializations to AgencyStaff or separate table
                    };
                    staffData.Add(staffDataItem);
                }
            }

            return Ok(staffData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<StaffData>> GetStaffMember(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var staffId))
                return BadRequest("Invalid staff ID format");

            // Get agency staff record
            var agencyStaff = await _agencyStaffRepository.GetByIdAsync(staffId);
            if (agencyStaff == null)
                return NotFound($"Staff member with ID {id} not found");

            // Get user details from AspNetUsers
            var user = await _userManager.FindByIdAsync(agencyStaff.UserId);
            if (user == null)
                return NotFound($"User not found for staff member {id}");

            var staffData = new StaffData
            {
                Id = agencyStaff.Id.ToString(),
                FullName = user.UserName ?? user.Email,
                Role = agencyStaff.Role,
                Email = user.Email,
                Phone = user.PhoneNumber ?? "",
                HireDate = agencyStaff.HireDate,
                HourlyRate = 0, // TODO: Add hourly rate to AgencyStaff or separate table
                Status = agencyStaff.Status.ToString(),
                Address = "", // TODO: Add address to user profile or separate table
                Notes = "", // TODO: Add notes to AgencyStaff or separate table
                Languages = new List<string>(), // TODO: Add languages to AgencyStaff or separate table
                Specializations = "" // TODO: Add specializations to AgencyStaff or separate table
            };

            return Ok(staffData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting staff member {StaffId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost]
    public async Task<ActionResult<StaffData>> CreateStaffMember([FromBody] CreateStaffRequest request)
    {
        try
        {
            // Get the first agency from the database (TODO: Get from current user context)
            // For now, we'll use the first available agency
            var agencies = await _agencyRepository.GetAllAsync();
            var firstAgency = agencies.FirstOrDefault();
            if (firstAgency == null)
            {
                return BadRequest(new { error = "No agencies found. Please create an agency first." });
            }
            var defaultAgencyId = firstAgency.Id;

            // Check if user with this email already exists
            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                // Check if user already has a role in this agency
                if (await _agencyStaffRepository.UserHasRoleInAgencyAsync(existingUser.Id, defaultAgencyId, request.Role))
                {
                    return BadRequest(new { error = $"User with email '{request.Email}' already has role '{request.Role}' in this agency" });
                }
            }

            // Create new user if they don't exist
            IdentityUser newUser;
            if (existingUser == null)
            {
                newUser = new IdentityUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    EmailConfirmed = true, // Since this is staff creation, we can assume email is confirmed
                    PhoneNumber = request.Phone,
                    PhoneNumberConfirmed = false
                };

                var createUserResult = await _userManager.CreateAsync(newUser);
                if (!createUserResult.Succeeded)
                {
                    var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                    return BadRequest(new { error = $"Failed to create user: {errors}" });
                }
            }
            else
            {
                newUser = existingUser;
            }

            // Create AgencyStaff record
            // Use the same defaultAgencyId from above
            var agencyStaff = AgencyStaff.Create(
                agencyId: defaultAgencyId,
                userId: newUser.Id,
                role: request.Role,
                hireDate: request.HireDate
            );

            var createdAgencyStaff = await _agencyStaffRepository.AddAsync(agencyStaff);

            // Create StaffData response
            var createdStaff = new StaffData
            {
                Id = createdAgencyStaff.Id.ToString(),
                FullName = request.FullName,
                Email = request.Email,
                Role = request.Role,
                Phone = request.Phone,
                HireDate = request.HireDate,
                HourlyRate = request.HourlyRate,
                Status = createdAgencyStaff.Status.ToString(),
                Address = request.Address,
                Notes = request.Notes,
                Languages = request.Languages,
                Specializations = request.Specializations
            };

            return CreatedAtAction(nameof(GetStaffMember), new { id = createdStaff.Id }, createdStaff);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff member");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<StaffData>> UpdateStaffMember(string id, [FromBody] UpdateStaffRequest request)
    {
        try
        {
            if (!Guid.TryParse(id, out var staffId))
                return BadRequest("Invalid staff ID format");

            // Get existing agency staff record
            var agencyStaff = await _agencyStaffRepository.GetByIdAsync(staffId);
            if (agencyStaff == null)
                return NotFound($"Staff member with ID {id} not found");

            // Get user details from AspNetUsers
            var user = await _userManager.FindByIdAsync(agencyStaff.UserId);
            if (user == null)
                return NotFound($"User not found for staff member {id}");

            // Update user information
            user.UserName = request.FullName;
            user.Email = request.Email;
            user.PhoneNumber = request.Phone;

            var updateUserResult = await _userManager.UpdateAsync(user);
            if (!updateUserResult.Succeeded)
            {
                var errors = string.Join(", ", updateUserResult.Errors.Select(e => e.Description));
                return BadRequest(new { error = $"Failed to update user: {errors}" });
            }

            // Update agency staff information
            agencyStaff.UpdateRole(request.Role);
            // Note: AgencyStaff doesn't have hourly rate, so we can't update it here
            // TODO: Consider adding hourly rate to AgencyStaff or create a separate StaffDetails table

            await _agencyStaffRepository.UpdateAsync(agencyStaff);

            // Return updated staff data
            var updatedStaff = new StaffData
            {
                Id = agencyStaff.Id.ToString(),
                FullName = user.UserName ?? user.Email,
                Role = agencyStaff.Role,
                Email = user.Email,
                Phone = user.PhoneNumber ?? "",
                HireDate = agencyStaff.HireDate,
                HourlyRate = request.HourlyRate,
                Status = agencyStaff.Status.ToString(),
                Address = request.Address,
                Notes = request.Notes,
                Languages = request.Languages,
                Specializations = request.Specializations
            };

            return Ok(updatedStaff);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating staff member {StaffId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteStaffMember(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var staffId))
                return BadRequest("Invalid staff ID format");

            // Get existing agency staff record
            var agencyStaff = await _agencyStaffRepository.GetByIdAsync(staffId);
            if (agencyStaff == null)
                return NotFound($"Staff member with ID {id} not found");

            // Delete the agency staff record
            await _agencyStaffRepository.DeleteAsync(agencyStaff);

            // Note: We don't delete the user from AspNetUsers as they might have other roles
            // or be used by other parts of the system

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting staff member {StaffId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}/activate")]
    public async Task<ActionResult<bool>> ActivateStaffMember(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var staffId))
                return BadRequest("Invalid staff ID format");

            // Get existing agency staff record
            var agencyStaff = await _agencyStaffRepository.GetByIdAsync(staffId);
            if (agencyStaff == null)
                return NotFound($"Staff member with ID {id} not found");

            // Activate the staff member
            agencyStaff.UpdateStatus(StaffStatus.Active);
            await _agencyStaffRepository.UpdateAsync(agencyStaff);

            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error activating staff member {StaffId}", id);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("{id}/deactivate")]
    public async Task<ActionResult<bool>> DeactivateStaffMember(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var staffId))
                return BadRequest("Invalid staff ID format");

            // Get existing agency staff record
            var agencyStaff = await _agencyStaffRepository.GetByIdAsync(staffId);
            if (agencyStaff == null)
                return NotFound($"Staff member with ID {id} not found");

            // Deactivate the staff member
            agencyStaff.UpdateStatus(StaffStatus.Inactive);
            await _agencyStaffRepository.UpdateAsync(agencyStaff);

            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deactivating staff member {StaffId}", id);
            return StatusCode(500, "Internal server error");
        }
    }


}
