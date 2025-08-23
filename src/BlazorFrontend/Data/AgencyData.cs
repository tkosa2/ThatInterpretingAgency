namespace BlazorFrontend.Data;

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
