using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace ThatInterpretingAgency.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult> GetNotifications([FromQuery] Guid? agencyId, [FromQuery] string? userId, [FromQuery] string? status)
    {
        // TODO: Implement GetNotificationsQuery
        return Ok(new { message = "Notifications retrieved successfully" });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult> GetNotification(Guid id)
    {
        // TODO: Implement GetNotificationQuery
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult> CreateNotification([FromBody] object createCommand)
    {
        // TODO: Implement CreateNotificationCommand
        return Ok(new { message = "Notification created successfully" });
    }

    [HttpPut("{id:guid}/mark-as-read")]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        // TODO: Implement MarkNotificationAsReadCommand
        return Ok(new { message = "Notification marked as read" });
    }

    [HttpPut("{id:guid}/mark-as-sent")]
    public async Task<ActionResult> MarkAsSent(Guid id)
    {
        // TODO: Implement MarkNotificationAsSentCommand
        return Ok(new { message = "Notification marked as sent" });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteNotification(Guid id)
    {
        // TODO: Implement DeleteNotificationCommand
        return Ok(new { message = "Notification deleted successfully" });
    }

    [HttpGet("templates")]
    public async Task<ActionResult> GetNotificationTemplates()
    {
        // TODO: Implement GetNotificationTemplatesQuery
        return Ok(new { message = "Notification templates retrieved successfully" });
    }

    [HttpPost("send-bulk")]
    public async Task<ActionResult> SendBulkNotifications([FromBody] object bulkCommand)
    {
        // TODO: Implement SendBulkNotificationsCommand
        return Ok(new { message = "Bulk notifications sent successfully" });
    }
}
