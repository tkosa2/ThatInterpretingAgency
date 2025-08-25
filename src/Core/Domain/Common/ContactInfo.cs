namespace ThatInterpretingAgency.Core.Domain.Common;

public class ContactInfo : ValueObject
{
    public string Name { get; private set; } = string.Empty;
    public string? ContactPerson { get; private set; }
    public string? Email { get; private set; }
    public string? PhoneNumber { get; private set; }
    public string? Address { get; private set; }
    public string? City { get; private set; }
    public string? State { get; private set; }
    public string? ZipCode { get; private set; }
    public string? Country { get; private set; }
    public string? Website { get; private set; }

    private ContactInfo() { }

    public static ContactInfo Create(
        string name,
        string? contactPerson = null,
        string? email = null,
        string? phoneNumber = null,
        string? address = null,
        string? city = null,
        string? state = null,
        string? zipCode = null,
        string? country = null,
        string? website = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        return new ContactInfo
        {
            Name = name.Trim(),
            ContactPerson = contactPerson?.Trim(),
            Email = email?.Trim(),
            PhoneNumber = phoneNumber?.Trim(),
            Address = address?.Trim(),
            City = city?.Trim(),
            State = state?.Trim(),
            ZipCode = zipCode?.Trim(),
            Country = country?.Trim(),
            Website = website?.Trim()
        };
    }

    public static ContactInfo CreateDefault() => Create("Default");

    public void UpdateContactInfo(
        string? contactPerson = null,
        string? email = null,
        string? phoneNumber = null,
        string? address = null,
        string? city = null,
        string? state = null,
        string? zipCode = null,
        string? country = null,
        string? website = null)
    {
        ContactPerson = contactPerson?.Trim();
        Email = email?.Trim();
        PhoneNumber = phoneNumber?.Trim();
        Address = address?.Trim();
        City = city?.Trim();
        State = state?.Trim();
        ZipCode = zipCode?.Trim();
        Country = country?.Trim();
        Website = website?.Trim();
    }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Name = name.Trim();
    }

    public string FullAddress
    {
        get
        {
            var parts = new List<string>();
            
            if (!string.IsNullOrWhiteSpace(Address))
                parts.Add(Address);
            if (!string.IsNullOrWhiteSpace(City))
                parts.Add(City);
            if (!string.IsNullOrWhiteSpace(State))
                parts.Add(State);
            if (!string.IsNullOrWhiteSpace(ZipCode))
                parts.Add(ZipCode);
            if (!string.IsNullOrWhiteSpace(Country))
                parts.Add(Country);

            return string.Join(", ", parts);
        }
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
        yield return ContactPerson ?? string.Empty;
        yield return Email ?? string.Empty;
        yield return PhoneNumber ?? string.Empty;
        yield return Address ?? string.Empty;
        yield return City ?? string.Empty;
        yield return State ?? string.Empty;
        yield return ZipCode ?? string.Empty;
        yield return Country ?? string.Empty;
        yield return Website ?? string.Empty;
    }
}
