using Microsoft.AspNetCore.Mvc;
using MediatR;
using ThatInterpretingAgency.Core.Application.Commands.CreateInvoice;

namespace ThatInterpretingAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BillingController : ControllerBase
{
    private readonly IMediator _mediator;

    public BillingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("invoices")]
    public async Task<ActionResult<CreateInvoiceResponse>> CreateInvoice([FromBody] CreateInvoiceCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetInvoice), new { id = result.InvoiceId }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = "An error occurred while creating the invoice" });
        }
    }

    [HttpGet("invoices/{id:guid}")]
    public async Task<ActionResult<CreateInvoiceResponse>> GetInvoice(Guid id)
    {
        // TODO: Implement GetInvoiceQuery
        return NotFound();
    }

    [HttpGet("invoices")]
    public async Task<ActionResult<IEnumerable<CreateInvoiceResponse>>> GetInvoices()
    {
        // TODO: Implement GetInvoicesQuery
        return Ok(new List<CreateInvoiceResponse>());
    }

    [HttpPost("invoices/{id:guid}/send")]
    public async Task<ActionResult> SendInvoice(Guid id)
    {
        // TODO: Implement SendInvoiceCommand
        return Ok(new { message = "Invoice sent successfully" });
    }

    [HttpPost("invoices/{id:guid}/mark-as-paid")]
    public async Task<ActionResult> MarkInvoiceAsPaid(Guid id)
    {
        // TODO: Implement MarkInvoiceAsPaidCommand
        return Ok(new { message = "Invoice marked as paid" });
    }
}
