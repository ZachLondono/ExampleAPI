using ExampleAPI.Companies.Domain;
using Xunit;
using FluentAssertions;
using System;

namespace ExampleAPI_UnitTests;

public class CompanyTests {

    [Fact]
    public void Should_CreateNewOrderedItem() {

        // Arrange
        string newname = "New Name";
        Address address = new() {
            Line1 = "A",
            Line2 = "A",
            City = "A",
            State = "A",
            Zip = "A",
        };

        // Act
        var company = Company.Create(newname, address);

        // Assert
        company.Name.Should().Be(newname);
        company.Address.Should().Be(address);

    }

    [Fact]
    public void Should_SetCompanyName() {

        // Arrange 
        var company = new Company(Guid.NewGuid(), 0, "New Company", new());
        var newName = "New Name";

        // Act
        company.SetName(newName);

        // Assert
        Assert.Equal(newName, company.Name);

    }

    [Fact]
    public void Should_CreateEvent_WhenSetingCompanyName() {

        // Arrange 
        var company = new Company(Guid.NewGuid(), 0, "New Company", new());
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
        var company = new Company(Guid.NewGuid(), 0, "New Company", new());
        var newAddr = new Address("A", "B", "C", "D", "E");

        // Act
        company.SetAddress(newAddr);

        // Assert
        Assert.Equal(newAddr, company.Address);

    }

    [Fact]
    public void Should_CreateEvent_WhenSetingCompanyAddress() {
        
        // Arrange 
        var company = new Company(Guid.NewGuid(), 0, "New Company", new());
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
