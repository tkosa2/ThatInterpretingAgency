namespace ThatInterpretingAgency.Core.DTOs;

public class InterpreterData
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public List<string> Languages { get; set; } = new();
    public string Specializations { get; set; } = "";
    public decimal HourlyRate { get; set; }
    public string Status { get; set; } = "Active";
}

public class ClientData
{
    public string Id { get; set; } = "";
    public string OrganizationName { get; set; } = "";
    public string ContactPerson { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public List<string> PreferredLanguages { get; set; } = new();
    public string BillingAddress { get; set; } = "";
    public string Status { get; set; } = "Active";
}

public class InvoiceData
{
    public string Id { get; set; } = "";
    public string ClientId { get; set; } = "";
    public string AppointmentId { get; set; } = "";
    public decimal Amount { get; set; }
    public string Status { get; set; } = "Draft";
    public DateTime DueDate { get; set; } = DateTime.Now.AddDays(30);
    public string Description { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class NotificationData
{
    public string Id { get; set; } = "";
    public string Type { get; set; } = "Email";
    public string RecipientId { get; set; } = "";
    public string Priority { get; set; } = "Normal";
    public string Subject { get; set; } = "";
    public string Message { get; set; } = "";
    public DateTime? ScheduledTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public class AppointmentData
{
    public string Id { get; set; } = "";
    public string Language { get; set; } = "";
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "Scheduled";
}
