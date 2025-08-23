namespace BlazorFrontend.Data;

public class StaffData
{
    public string Id { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateTime HireDate { get; set; } = DateTime.Now;
    public decimal HourlyRate { get; set; }
    public string Status { get; set; } = "Active";
    public string Address { get; set; } = "";
    public string Notes { get; set; } = "";
    public List<string> Languages { get; set; } = new();
    public string Specializations { get; set; } = "";
}
