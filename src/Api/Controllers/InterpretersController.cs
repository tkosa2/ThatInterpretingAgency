using Microsoft.AspNetCore.Mvc;
using MediatR;
using ThatInterpretingAgency.Core.Application.Commands.CreateInterpreter;

namespace ThatInterpretingAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InterpretersController : ControllerBase
{
    private readonly IMediator _mediator;

    public InterpretersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateInterpreterResponse>> CreateInterpreter([FromBody] CreateInterpreterCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetInterpreter), new { id = result.InterpreterId }, result);
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
            return StatusCode(500, new { error = "An error occurred while creating the interpreter" });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CreateInterpreterResponse>> GetInterpreter(Guid id)
    {
        // TODO: Implement GetInterpreterQuery
        return NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CreateInterpreterResponse>>> GetInterpreters([FromQuery] Guid? agencyId)
    {
        // TODO: Implement GetInterpretersQuery
        return Ok(new List<CreateInterpreterResponse>());
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> UpdateInterpreter(Guid id, [FromBody] object updateCommand)
    {
        // TODO: Implement UpdateInterpreterCommand
        return Ok(new { message = "Interpreter updated successfully" });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteInterpreter(Guid id)
    {
        // TODO: Implement DeleteInterpreterCommand
        return Ok(new { message = "Interpreter deleted successfully" });
    }

    [HttpGet("{id:guid}/availability")]
    public async Task<ActionResult> GetInterpreterAvailability(Guid id, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        // TODO: Implement GetInterpreterAvailabilityQuery
        return Ok(new { message = "Availability retrieved successfully" });
    }

    [HttpPost("{id:guid}/availability")]
    public async Task<ActionResult> AddAvailabilitySlot(Guid id, [FromBody] object availabilityCommand)
    {
        // TODO: Implement AddAvailabilitySlotCommand
        return Ok(new { message = "Availability slot added successfully" });
    }
}
