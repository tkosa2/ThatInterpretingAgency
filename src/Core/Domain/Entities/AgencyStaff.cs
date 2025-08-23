using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class AgencyStaff : Entity
{
    public Guid AgencyId { get; private set; }
    public Guid UserId { get; private set; }
    public string Role { get; private set; } = string.Empty;
    public DateTime HireDate { get; private set; }
    public StaffStatus Status { get; private set; }

    private AgencyStaff() { }

    public static AgencyStaff Create(Guid agencyId, Guid userId, string role, DateTime hireDate)
    {
        if (string.IsNullOrWhiteSpace(role))
            throw new ArgumentException("Role cannot be empty", nameof(role));

        if (hireDate > DateTime.UtcNow)
            throw new ArgumentException("Hire date cannot be in the future", nameof(hireDate));

        return new AgencyStaff
        {
            AgencyId = agencyId,
            UserId = userId,
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
