using System;
using Xunit;
using ThatInterpretingAgency.Core.Domain.Aggregates;

namespace ThatInterpretingAgency.Tests.UnitTests;

public class AgencyAggregateTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateAgency()
    {
        // Arrange
        var name = "Test Agency";
        var contactInfo = "test@agency.com";

        // Act
        var agency = AgencyAggregate.Create(name, contactInfo);

        // Assert
        Assert.NotNull(agency);
        Assert.Equal(name, agency.Name);
        Assert.Equal(contactInfo, agency.ContactInfo);
        Assert.Equal(AgencyStatus.Active, agency.Status);
        Assert.NotEqual(Guid.Empty, agency.Id);
        Assert.True(agency.CreatedAt > DateTime.UtcNow.AddMinutes(-1));
        Assert.True(agency.UpdatedAt > DateTime.UtcNow.AddMinutes(-1));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldThrowArgumentException(string invalidName)
    {
        // Arrange
        var contactInfo = "test@agency.com";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => AgencyAggregate.Create(invalidName!, contactInfo));
        Assert.Contains("name", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidContactInfo_ShouldThrowArgumentException(string invalidContactInfo)
    {
        // Arrange
        var name = "Test Agency";

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => AgencyAggregate.Create(name, invalidContactInfo!));
        Assert.Contains("contact info", exception.Message);
    }

    [Fact]
    public void UpdateContactInfo_WithValidData_ShouldUpdateContactInfo()
    {
        // Arrange
        var agency = AgencyAggregate.Create("Test Agency", "old@agency.com");
        var newContactInfo = "new@agency.com";
        var originalUpdatedAt = agency.UpdatedAt;

        // Act
        agency.UpdateContactInfo(newContactInfo);

        // Assert
        Assert.Equal(newContactInfo, agency.ContactInfo);
        Assert.True(agency.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void Deactivate_ShouldChangeStatusToInactive()
    {
        // Arrange
        var agency = AgencyAggregate.Create("Test Agency", "test@agency.com");
        var originalUpdatedAt = agency.UpdatedAt;

        // Act
        agency.Deactivate();

        // Assert
        Assert.Equal(AgencyStatus.Inactive, agency.Status);
        Assert.True(agency.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void Activate_ShouldChangeStatusToActive()
    {
        // Arrange
        var agency = AgencyAggregate.Create("Test Agency", "test@agency.com");
        agency.Deactivate();
        var originalUpdatedAt = agency.UpdatedAt;

        // Act
        agency.Activate();

        // Assert
        Assert.Equal(AgencyStatus.Active, agency.Status);
        Assert.True(agency.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public void AddStaff_WithValidData_ShouldAddStaff()
    {
        // Arrange
        var agency = AgencyAggregate.Create("Test Agency", "test@agency.com");
        var userId = Guid.NewGuid();
        var role = "Interpreter";
        var hireDate = DateTime.UtcNow.AddDays(-30);

        // Act
        agency.AddStaff(userId, role, hireDate);

        // Assert
        Assert.Single(agency.Staff);
        var staff = agency.Staff.First();
        Assert.Equal(userId, staff.UserId);
        Assert.Equal(role, staff.Role);
        Assert.Equal(hireDate, staff.HireDate);
    }

    [Fact]
    public void AddStaff_WithDuplicateRole_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var agency = AgencyAggregate.Create("Test Agency", "test@agency.com");
        var userId = Guid.NewGuid();
        var role = "Interpreter";
        var hireDate = DateTime.UtcNow.AddDays(-30);

        agency.AddStaff(userId, role, hireDate);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => 
            agency.AddStaff(userId, role, hireDate));
        Assert.Contains("already has role", exception.Message);
    }
}
