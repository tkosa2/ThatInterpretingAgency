using ThatInterpretingAgency.Core.Domain.Common;

namespace ThatInterpretingAgency.Core.Domain.Entities;

public class UserProfile : Entity
{
    public string UserId { get; private set; } = string.Empty; // This will be the AspNetUsers.Id (nvarchar(450))
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }
    public string? MailingAddress { get; private set; }
    public string? PhysicalAddress { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? ZipCode { get; private set; }
    public string? Country { get; private set; }

    private UserProfile() { }

    public static UserProfile Create(
        string userId,
        string firstName,
        string lastName,
        string? middleName = null,
        string? mailingAddress = null,
        string? physicalAddress = null,
        string? city = null,
        string? state = null,
        string? zipCode = null,
        string? country = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be empty", nameof(userId));

        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        return new UserProfile
        {
            UserId = userId.Trim(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            MiddleName = middleName?.Trim(),
            MailingAddress = mailingAddress?.Trim(),
            PhysicalAddress = physicalAddress?.Trim(),
            City = city?.Trim(),
            State = state?.Trim(),
            ZipCode = zipCode?.Trim(),
            Country = country?.Trim()
        };
    }

    public void UpdateName(string firstName, string lastName, string? middleName = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MiddleName = middleName?.Trim();
        UpdateTimestamp();
    }

    public void UpdateAddresses(
        string? mailingAddress = null,
        string? physicalAddress = null,
        string? city = null,
        string? state = null,
        string? zipCode = null,
        string? country = null)
    {
        MailingAddress = mailingAddress?.Trim();
        PhysicalAddress = physicalAddress?.Trim();
        City = city?.Trim();
        State = state?.Trim();
        ZipCode = zipCode?.Trim();
        Country = country?.Trim();
        UpdateTimestamp();
    }

    public string FullName => $"{FirstName} {LastName}".Trim();
    public string FullNameWithMiddle => $"{FirstName} {MiddleName} {LastName}".Trim();
}
