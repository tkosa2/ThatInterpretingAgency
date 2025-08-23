namespace BlazorFrontend.Data;

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

public class CreateStaffRequest
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateTime HireDate { get; set; } = DateTime.Now;
    public decimal HourlyRate { get; set; }
    public string Address { get; set; } = "";
    public string Notes { get; set; } = "";
    public List<string> Languages { get; set; } = new();
    public string Specializations { get; set; } = "";
}

public class UpdateStaffRequest
{
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateTime HireDate { get; set; }
    public decimal HourlyRate { get; set; }
    public string Address { get; set; } = "";
    public string Notes { get; set; } = "";
    public List<string> Languages { get; set; } = new();
    public string Specializations { get; set; } = "";
    public string Status { get; set; } = "";
}
