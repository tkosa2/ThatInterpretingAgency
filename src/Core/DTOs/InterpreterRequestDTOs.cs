namespace ThatInterpretingAgency.Core.DTOs;

public class InterpreterRequestData
{
    public string Id { get; set; } = "";
    public string AgencyId { get; set; } = "";
    public string RequestorId { get; set; } = "";
    public string AppointmentType { get; set; } = ""; // In-Person, Virtual
    public string? VirtualMeetingLink { get; set; }
    public string? Location { get; set; }
    public string? Mode { get; set; } // Consecutive, Simultaneous
    public string? Description { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Language { get; set; } = "";
    public string? SpecialInstructions { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Fulfilled, Cancelled
    public string? Division { get; set; }
    public string? Program { get; set; }
    public string? LniContact { get; set; }
    public string? DayOfEventContact { get; set; }
    public string? DayOfEventContactPhone { get; set; }
    public string? CostCode { get; set; }
    public string? InvoiceApprover { get; set; }
    public bool SupportingMaterials { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public string? RequestorName { get; set; }
    public string? OrganizationName { get; set; }
}

public class CreateInterpreterRequestRequest
{
    public string AgencyId { get; set; } = "";
    public string RequestorId { get; set; } = "";
    public string AppointmentType { get; set; } = ""; // In-Person, Virtual
    public string? VirtualMeetingLink { get; set; }
    public string? Location { get; set; }
    public string? Mode { get; set; } // Consecutive, Simultaneous
    public string? Description { get; set; }
    public DateTime RequestedDate { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Language { get; set; } = "";
    public string? SpecialInstructions { get; set; }
    public string? Division { get; set; }
    public string? Program { get; set; }
    public string? LniContact { get; set; }
    public string? DayOfEventContact { get; set; }
    public string? DayOfEventContactPhone { get; set; }
    public string? CostCode { get; set; }
    public string? InvoiceApprover { get; set; }
    public bool SupportingMaterials { get; set; }
}

public class UpdateInterpreterRequestStatusRequest
{
    public string Status { get; set; } = ""; // Pending, Approved, Rejected, Fulfilled, Cancelled
    public string? AppointmentId { get; set; } // Required when Status is 'Fulfilled'
    public string? Notes { get; set; }
}

public class InterpreterRequestFilter
{
    public string? AgencyId { get; set; }
    public string? RequestorId { get; set; }
    public string? Status { get; set; }
    public string? Language { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

