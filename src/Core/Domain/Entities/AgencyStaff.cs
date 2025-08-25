using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class AgencyStaff : Entity
{
    public Guid AgencyId { get; private set; }
    public string UserId { get; private set; } = string.Empty; // Changed to string to match AspNetUsers.Id
    public string Role { get; private set; } = string.Empty;
    public DateTime HireDate { get; private set; }
    public StaffStatus Status { get; private set; }

    // Navigation properties
    public virtual UserProfile UserProfile { get; private set; } = null!;

    private AgencyStaff() { }

    public static AgencyStaff Create(Guid agencyId, string userId, string role, DateTime hireDate)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty", nameof(role));

        if (hireDate > DateTime.UtcNow)
            throw new ArgumentException("Hire date cannot be in the future", nameof(hireDate));

        return new AgencyStaff
        {
            AgencyId = agencyId,
            UserId = userId.Trim(),
            Role = role.Trim(),
            HireDate = hireDate,
            Status = StaffStatus.Active
        };
    }

    public void UpdateRole(string newRole)
    {
        if (string.IsNullOrWhiteSpace(newRole))
            throw new ArgumentException("Role cannot be empty", nameof(newRole));

        Role = newRole.Trim();
        UpdateTimestamp();
    }

    public void UpdateStatus(StaffStatus newStatus)
    {
        Status = newStatus;
        UpdateTimestamp();
    }

    public void Terminate()
    {
        Status = StaffStatus.Terminated;
        UpdateTimestamp();
    }
}

public enum StaffStatus
{
    Active,
    Inactive,
    Suspended,
    Terminated
}
