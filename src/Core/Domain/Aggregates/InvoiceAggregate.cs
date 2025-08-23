using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Aggregates;

public class InvoiceAggregate : AggregateRoot
{
    public Guid AgencyId { get; private set; }
    public Guid ClientId { get; private set; }
    public Guid AppointmentId { get; private set; }
    public string QuickBooksInvoiceId { get; private set; } = string.Empty;
    public InvoiceStatus Status { get; private set; }
    public DateTime? DueDate { get; private set; }
    public decimal? Amount { get; private set; }
    public string? Currency { get; private set; }
    public string? Notes { get; private set; }

    private InvoiceAggregate() { }

    public static InvoiceAggregate Create(
        Guid agencyId, 
        Guid clientId, 
        Guid appointmentId, 
        string quickBooksInvoiceId,
        DateTime? dueDate = null,
        decimal? amount = null,
        string? currency = null,
        string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(quickBooksInvoiceId))
            throw new ArgumentException("QuickBooks invoice ID cannot be empty", nameof(quickBooksInvoiceId));

        var invoice = new InvoiceAggregate
        {
            AgencyId = agencyId,
            ClientId = clientId,
            AppointmentId = appointmentId,
            QuickBooksInvoiceId = quickBooksInvoiceId.Trim(),
            Status = InvoiceStatus.Draft,
            DueDate = dueDate,
            Amount = amount,
            Currency = currency?.Trim(),
            Notes = notes?.Trim()
        };

        return invoice;
    }

    public void UpdateQuickBooksReference(string quickBooksInvoiceId)
    {
        if (string.IsNullOrWhiteSpace(quickBooksInvoiceId))
            throw new ArgumentException("QuickBooks invoice ID cannot be empty", nameof(quickBooksInvoiceId));

        QuickBooksInvoiceId = quickBooksInvoiceId.Trim();
        UpdateTimestamp();
    }

    public void UpdateStatus(InvoiceStatus newStatus)
    {
        Status = newStatus;
        UpdateTimestamp();
    }

    public void UpdatePaymentDetails(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        Amount = amount;
        Currency = currency.Trim();
        UpdateTimestamp();
    }

    public void UpdateDueDate(DateTime dueDate)
    {
        if (dueDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Due date cannot be in the past", nameof(dueDate));

        DueDate = dueDate;
        UpdateTimestamp();
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes?.Trim();
        UpdateTimestamp();
    }

    public void MarkAsSent()
    {
        if (Status != InvoiceStatus.Draft)
            throw new InvalidOperationException("Only draft invoices can be marked as sent");

        Status = InvoiceStatus.Sent;
        UpdateTimestamp();
    }

    public void MarkAsPaid()
    {
        if (Status != InvoiceStatus.Sent)
            throw new ArgumentException("Only sent invoices can be marked as paid");

        Status = InvoiceStatus.Paid;
        UpdateTimestamp();
    }

    public void MarkAsOverdue()
    {
        if (Status != InvoiceStatus.Sent)
            throw new InvalidOperationException("Only sent invoices can be marked as overdue");

        if (DueDate.HasValue && DateTime.UtcNow.Date > DueDate.Value.Date)
        {
            Status = InvoiceStatus.Overdue;
            UpdateTimestamp();
        }
    }

    public bool IsOverdue => DueDate.HasValue && DateTime.UtcNow.Date > DueDate.Value.Date && Status == InvoiceStatus.Sent;
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
