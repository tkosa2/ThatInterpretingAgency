namespace BlazorFrontend.Data;

// BFF Agency DTOs that match the BFF API response structure
public class BffAgencySummaryDTO
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

public class BffAgencyDetailDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<BffAgencyStaffDTO> Staff { get; set; } = new();
    public List<BffInterpreterSummaryDTO> Interpreters { get; set; } = new();
    public List<BffClientSummaryDTO> Clients { get; set; } = new();
    public List<BffAppointmentSummaryDTO> RecentAppointments { get; set; } = new();
}

public class BffAgencyStaffDTO
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class BffInterpreterSummaryDTO
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Skills { get; set; } = string.Empty;
    public string Languages { get; set; } = string.Empty;
    public int AppointmentCount { get; set; }
    public decimal TotalEarnings { get; set; }
}

public class BffClientSummaryDTO
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int AppointmentCount { get; set; }
    public decimal TotalSpent { get; set; }
}

public class BffAppointmentSummaryDTO
{
    public Guid Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string InterpreterName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

// BFF Dashboard DTOs
public class BffDashboardDTO
{
    public int TotalAgencies { get; set; }
    public int TotalInterpreters { get; set; }
    public int TotalClients { get; set; }
    public int TotalAppointments { get; set; }
    public int PendingInterpreterRequests { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<BffRecentAppointmentDTO> RecentAppointments { get; set; } = new();
    public List<BffUpcomingAppointmentDTO> UpcomingAppointments { get; set; } = new();
    public List<BffInterpreterRequestDTO> RecentRequests { get; set; } = new();
}

public class BffRecentAppointmentDTO
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

public class BffUpcomingAppointmentDTO
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

public class BffInterpreterRequestDTO
{
    public Guid Id { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public DateTime RequestedDate { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

