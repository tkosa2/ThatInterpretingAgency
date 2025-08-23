using ThatInterpretingAgency.Core.Domain.Common;
using ThatInterpretingAgency.Core.Domain.Aggregates;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class InterpreterRequest : Entity
{
    public Guid AgencyId { get; private set; }
    public Guid RequestorId { get; private set; }
    public string AppointmentType { get; private set; } = string.Empty; // In-Person, Virtual
    public string? VirtualMeetingLink { get; private set; }
    public string? Location { get; private set; }
    public string? Mode { get; private set; } // Consecutive, Simultaneous
    public string? Description { get; private set; }
    public DateTime RequestedDate { get; private set; }
    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public string Language { get; private set; } = string.Empty;
    public string? SpecialInstructions { get; private set; }
    public string Status { get; private set; } = "Pending"; // Pending, Approved, Rejected, Fulfilled, Cancelled
    public string? Division { get; private set; }
    public string? Program { get; private set; }
    public string? LniContact { get; private set; }
    public string? DayOfEventContact { get; private set; }
    public string? DayOfEventContactPhone { get; private set; }
    public string? CostCode { get; private set; }
    public string? InvoiceApprover { get; private set; }
    public bool SupportingMaterials { get; private set; }
    public Guid? AppointmentId { get; private set; } // Link to created appointment when fulfilled

    // Navigation properties
    public virtual AgencyAggregate Agency { get; private set; } = null!;
    public virtual Client Requestor { get; private set; } = null!;
    public virtual AppointmentAggregate? Appointment { get; private set; }

    private InterpreterRequest() { } // For EF Core

    public InterpreterRequest(
        Guid agencyId,
        Guid requestorId,
        string appointmentType,
        DateTime requestedDate,
        DateTime startTime,
        DateTime endTime,
        string language,
        string? description = null,
        string? mode = null,
        string? location = null,
        string? virtualMeetingLink = null,
        string? specialInstructions = null,
        string? division = null,
        string? program = null,
        string? lniContact = null,
        string? dayOfEventContact = null,
        string? dayOfEventContactPhone = null,
        string? costCode = null,
        string? invoiceApprover = null,
        bool supportingMaterials = false)
    {
        AgencyId = agencyId;
        RequestorId = requestorId;
        AppointmentType = appointmentType;
        RequestedDate = requestedDate;
        StartTime = startTime;
        EndTime = endTime;
        Language = language;
        Description = description;
        Mode = mode;
        Location = location;
        VirtualMeetingLink = virtualMeetingLink;
        SpecialInstructions = specialInstructions;
        Division = division;
        Program = program;
        LniContact = lniContact;
        DayOfEventContact = dayOfEventContact;
        DayOfEventContactPhone = dayOfEventContactPhone;
        CostCode = costCode;
        InvoiceApprover = invoiceApprover;
        SupportingMaterials = supportingMaterials;
        Status = "Pending";
    }

    public void UpdateStatus(string newStatus, Guid? appointmentId = null)
    {
        if (newStatus == "Fulfilled" && appointmentId == null)
        {
            throw new InvalidOperationException("AppointmentId is required when status is set to Fulfilled");
        }

        Status = newStatus;
        if (appointmentId.HasValue)
        {
            AppointmentId = appointmentId.Value;
        }

        UpdateTimestamp();
    }

    public void Cancel()
    {
        if (Status == "Fulfilled")
        {
            throw new InvalidOperationException("Cannot cancel a fulfilled request");
        }

        Status = "Cancelled";
        UpdateTimestamp();
    }

    public void UpdateDetails(
        string? description = null,
        string? specialInstructions = null,
        string? division = null,
        string? program = null,
        string? lniContact = null,
        string? dayOfEventContact = null,
        string? dayOfEventContactPhone = null,
        string? costCode = null,
        string? invoiceApprover = null,
        bool? supportingMaterials = null)
    {
        if (description != null) Description = description;
        if (specialInstructions != null) SpecialInstructions = specialInstructions;
        if (division != null) Division = division;
        if (program != null) Program = program;
        if (lniContact != null) LniContact = lniContact;
        if (dayOfEventContact != null) DayOfEventContact = dayOfEventContact;
        if (dayOfEventContactPhone != null) DayOfEventContactPhone = dayOfEventContactPhone;
        if (costCode != null) CostCode = costCode;
        if (invoiceApprover != null) InvoiceApprover = invoiceApprover;
        if (supportingMaterials.HasValue) SupportingMaterials = supportingMaterials.Value;

        UpdateTimestamp();
    }

    public bool CanBeApproved => Status == "Pending";
    public bool CanBeRejected => Status == "Pending";
    public bool CanBeCancelled => Status == "Pending" || Status == "Approved";
    public bool CanBeFulfilled => Status == "Approved";
}
