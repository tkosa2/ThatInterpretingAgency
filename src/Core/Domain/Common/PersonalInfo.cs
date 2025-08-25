using System.ComponentModel.DataAnnotations;

namespace ThatInterpretingAgency.Core.Domain.Common;

public class PersonalInfo : ValueObject
{
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string? MiddleName { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? ZipCode { get; private set; }
    public string? Country { get; private set; }

    private PersonalInfo() { }

    public static PersonalInfo Create(
        string firstName,
        string lastName,
        string? middleName = null,
        string? email = null,
        string? phoneNumber = null,
        string? address = null,
        string? city = null,
        string? state = null,
        string? zipCode = null,
        string? country = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("First name cannot be empty", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Last name cannot be empty", nameof(lastName));

        return new PersonalInfo
        {
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            MiddleName = middleName?.Trim(),
            Email = email?.Trim(),
            PhoneNumber = phoneNumber?.Trim(),
            Address = address?.Trim(),
            City = city?.Trim(),
            State = state?.Trim(),
            ZipCode = zipCode?.Trim(),
            Country = country?.Trim()
        };
    }

    public static PersonalInfo CreateDefault() => Create("Default", "Default");

    public void UpdateContactInfo(
        string? email = null,
        string? phoneNumber = null,
        string? address = null,
        string? city = null,
        string? state = null,
        string? zipCode = null,
        string? country = null)
    {
        Email = email?.Trim();
        PhoneNumber = phoneNumber?.Trim();
        Address = address?.Trim();
        City = city?.Trim();
        State = state?.Trim();
        ZipCode = zipCode?.Trim();
        Country = country?.Trim();
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
    }

    public string FullName => $"{FirstName} {LastName}".Trim();
    public string FullNameWithMiddle => $"{FirstName} {MiddleName} {LastName}".Trim();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return LastName;
        yield return MiddleName ?? string.Empty;
        yield return Email ?? string.Empty;
        yield return PhoneNumber ?? string.Empty;
        yield return Address ?? string.Empty;
        yield return City ?? string.Empty;
        yield return State ?? string.Empty;
        yield return ZipCode ?? string.Empty;
        yield return Country ?? string.Empty;
    }
}
