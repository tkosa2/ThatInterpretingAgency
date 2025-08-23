namespace ThatInterpretingAgency.Core.DTOs;

public class AgencyData
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string ContactInfo { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Status { get; set; } = "Active";
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}

public class AgencyStats
{
    public int TotalStaff { get; set; }
    public int ActiveInterpreters { get; set; }
    public int TotalClients { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int TotalAppointments { get; set; }
    public int PendingInvoices { get; set; }
}

public class CreateAgencyRequest
{
    public string Name { get; set; } = "";
    public string ContactInfo { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
}

public class UpdateAgencyRequest
{
    public string Name { get; set; } = "";
    public string ContactInfo { get; set; } = "";
    public string Address { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Status { get; set; } = "";
}
