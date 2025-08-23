using System;
using MediatR;

namespace ThatInterpretingAgency.Core.Application.Commands.CreateInvoice;

public record CreateInvoiceCommand : IRequest<CreateInvoiceResponse>
{
    public Guid AgencyId { get; init; }
    public Guid ClientId { get; init; }
    public Guid AppointmentId { get; init; }
    public string QuickBooksInvoiceId { get; init; } = string.Empty;
    public DateTime? DueDate { get; init; }
    public decimal? Amount { get; init; }
    public string? Currency { get; init; }
    public string? Notes { get; init; }
}

public record CreateInvoiceResponse
{
    public Guid InvoiceId { get; init; }
    public Guid AgencyId { get; init; }
    public Guid ClientId { get; init; }
    public Guid AppointmentId { get; init; }
    public string QuickBooksInvoiceId { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}
