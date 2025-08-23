using Microsoft.AspNetCore.Mvc;
using MediatR;
using ThatInterpretingAgency.Core.Application.Commands.BookAppointment;
using ThatInterpretingAgency.Core.Application.Queries.GetAvailableInterpreters;

namespace ThatInterpretingAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AppointmentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult<BookAppointmentResponse>> BookAppointment([FromBody] BookAppointmentCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAppointment), new { id = result.AppointmentId }, result);
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
            return StatusCode(500, new { error = "An error occurred while booking the appointment" });
        }
    }

    [HttpGet("available-interpreters")]
    public async Task<ActionResult<GetAvailableInterpretersResponse>> GetAvailableInterpreters([FromQuery] GetAvailableInterpretersQuery query)
    {
        try
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while retrieving available interpreters" });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BookAppointmentResponse>> GetAppointment(Guid id)
    {
        // TODO: Implement GetAppointmentQuery
        return NotFound();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookAppointmentResponse>>> GetAppointments()
    {
        // TODO: Implement GetAppointmentsQuery
        return Ok(new List<BookAppointmentResponse>());
    }
}
