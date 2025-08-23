using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThatInterpretingAgency.Core.Application.Common;

public interface IQuickBooksService
{
    Task<string> CreateInvoiceAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default);
    Task<InvoiceStatus> GetInvoiceStatusAsync(string quickBooksInvoiceId, CancellationToken cancellationToken = default);
    Task<bool> SendInvoiceAsync(string quickBooksInvoiceId, CancellationToken cancellationToken = default);
    Task<bool> ProcessPaymentAsync(string quickBooksInvoiceId, decimal amount, CancellationToken cancellationToken = default);
}

public record CreateInvoiceRequest
{
    public Guid ClientId { get; init; }
    public Guid AppointmentId { get; init; }
    public string ClientName { get; init; } = string.Empty;
    public string ClientEmail { get; init; } = string.Empty;
    public DateTime AppointmentDate { get; init; }
    public TimeSpan Duration { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "USD";
    public string? Description { get; init; }
}

public enum InvoiceStatus
{
    Draft,
    Sent,
    Paid,
    Overdue,
    Cancelled,
    Void
}
