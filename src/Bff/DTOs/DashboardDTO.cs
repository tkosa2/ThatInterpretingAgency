namespace Bff.DTOs;

public class DashboardDTO
{
    public int TotalAgencies { get; set; }
    public int TotalInterpreters { get; set; }
    public int TotalClients { get; set; }
    public int TotalAppointments { get; set; }
    public int PendingInterpreterRequests { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<RecentAppointmentDTO> RecentAppointments { get; set; } = new();
    public List<UpcomingAppointmentDTO> UpcomingAppointments { get; set; } = new();
    public List<InterpreterRequestDTO> RecentRequests { get; set; } = new();
}

public class RecentAppointmentDTO
{
    public Guid Id { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public string InterpreterName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class UpcomingAppointmentDTO
{
    public Guid Id { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public string InterpreterName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
}

public class InterpreterRequestDTO
{
    public Guid Id { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
