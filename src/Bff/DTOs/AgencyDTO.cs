namespace Bff.DTOs;

public class AgencySummaryDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int InterpreterCount { get; set; }
    public int ClientCount { get; set; }
    public int AppointmentCount { get; set; }
    public decimal TotalRevenue { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AgencyDetailDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<AgencyStaffDTO> Staff { get; set; } = new();
    public List<InterpreterSummaryDTO> Interpreters { get; set; } = new();
    public List<ClientSummaryDTO> Clients { get; set; } = new();
    public List<AppointmentSummaryDTO> RecentAppointments { get; set; } = new();
}

public class AgencyStaffDTO
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class InterpreterSummaryDTO
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public string Languages { get; set; } = string.Empty;
    public int AppointmentCount { get; set; }
    public decimal TotalEarnings { get; set; }
}

public class ClientSummaryDTO
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int AppointmentCount { get; set; }
    public decimal TotalSpent { get; set; }
}

public class AppointmentSummaryDTO
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string InterpreterName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
