using Microsoft.AspNetCore.Mvc;
using MediatR;
using ThatInterpretingAgency.Core.Application.Commands.CreateAgency;
using ThatInterpretingAgency.Core.Application.Commands.UpdateAgency;
using ThatInterpretingAgency.Core.Application.Commands.DeleteAgency;
using ThatInterpretingAgency.Core.Application.Queries.GetAgency;
using ThatInterpretingAgency.Core.Application.Queries.GetAgencies;
using ThatInterpretingAgency.Core.Application.Queries.GetAgencyStats;
using ThatInterpretingAgency.Core.DTOs;
using System;

namespace ThatInterpretingAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgenciesController : ControllerBase
{
    private readonly IMediator _mediator;

    public AgenciesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateAgencyResponse>> CreateAgency([FromBody] CreateAgencyCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAgency), new { id = result.AgencyId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while creating the agency" });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AgencyData>> GetAgency(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var agencyId))
            {
                return BadRequest(new { error = "Invalid agency ID format" });
            }

            var query = new GetAgencyQuery { Id = agencyId };
            var agency = await _mediator.Send(query);

            if (agency == null)
            {
                return NotFound(new { error = "Agency not found" });
            }

            return Ok(agency);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving the agency" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AgencyData>>> GetAgencies()
    {
        try
        {
            var query = new GetAgenciesQuery();
            var agencies = await _mediator.Send(query);
            return Ok(agencies);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving agencies" });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AgencyData>> UpdateAgency(string id, [FromBody] UpdateAgencyRequest request)
    {
        try
        {
            if (!Guid.TryParse(id, out var agencyId))
            {
                return BadRequest(new { error = "Invalid agency ID format" });
            }

            var command = new UpdateAgencyCommand
            {
                Id = agencyId,
                Name = request.Name,
                ContactInfo = request.ContactInfo,
                Address = request.Address,
                Phone = request.Phone,
                Email = request.Email,
                Status = request.Status
            };

            var updatedAgency = await _mediator.Send(command);
            return Ok(updatedAgency);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while updating the agency" });
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAgency(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var agencyId))
            {
                return BadRequest(new { error = "Invalid agency ID format" });
            }

            var command = new DeleteAgencyCommand { Id = agencyId };
            var success = await _mediator.Send(command);

            if (!success)
            {
                return NotFound(new { error = "Agency not found" });
            }

            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while deleting the agency" });
        }
    }

    [HttpGet("{id}/stats")]
    public async Task<ActionResult<AgencyStats>> GetAgencyStats(string id)
    {
        try
        {
            if (!Guid.TryParse(id, out var agencyId))
            {
                return BadRequest(new { error = "Invalid agency ID format" });
            }

            var query = new GetAgencyStatsQuery { Id = agencyId };
            var stats = await _mediator.Send(query);
            return Ok(stats);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving agency stats" });
        }
    }
}
