using ExampleAPI.Companies.Domain;
using Xunit;
using FluentAssertions;

namespace ExampleAPI_UnitTests;

public class CompanyTests {

    [Fact]
    public void Should_SetCompanyName() {

        // Arrange 
        var company = new Company(0, "New Company", null);
        var newName = "New Name";

        // Act
        company.SetName(newName);

        // Assert
        Assert.Equal(newName, company.Name);

    }

    [Fact]
    public void Should_CreateEvent_WhenSetingCompanyName() {

        // Arrange 
        var company = new Company(0, "New Company", null);
        var newName = "New Name";

        // Act
        company.SetName(newName);

        // Assert
        Assert.Contains(company.Events, (e) => {
            if (e is Events.NameChangedEvent nameChange) {
                return nameChange.Name.Equals(newName) && nameChange.CompanyId == company.Id;
            }

            return false;
        });

    }

    [Fact]
    public void Should_SetCompanyAddress() {

        // Arrange 
        var company = new Company(0, "New Company", null);
        var newAddr = new Address("A", "B", "C", "D", "E");

        // Act
        company.SetAddress(newAddr);

        // Assert
        Assert.Equal(newAddr, company.Address);

    }

    [Fact]
    public void Should_CreateEvent_WhenSetingCompanyAddress() {
        
        // Arrange 
        var company = new Company(0, "New Company", null);
        var newAddr = new Address("A", "B", "C", "D", "E");

        // Act
        company.SetAddress(newAddr);

        // Assert
        Assert.Contains(company.Events, (e) => {
            if (e is Events.AddressChangedEvent addrChanged) {
                return addrChanged.NewAddress.Equals(newAddr) && addrChanged.CompanyId == company.Id;
            }

            return false;
        });

    }


}
