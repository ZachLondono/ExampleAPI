using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Companies.Commands;
using ExampleAPI.Sales.Companies.Domain;
using ExampleAPI.Sales.Companies.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ExampleAPI_UnitTests;

public class CompanyCommandTests {

    [Fact]
    public async Task Create_Should_ReturnCompanyAsync() {

        // Arrange
        var expected_name = "New Company Name";
        var expected_addr = new AddressDTO() {
            Line1 = "A",
            Line2 = "B",
            City = "C",
            State = "D",
            Zip = "E"
        };

        var mock = new Mock<IRepository<Company>>();
        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new Create.Command(context, new() {
            Name = expected_name,
            Address = expected_addr
        });
        var handler = new Create.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        var response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<CreatedResult>();
        
        var result = response as CreatedResult;
        result.Should().NotBeNull();
        result!.Value.Should().BeOfType<CompanyDTO>();
        
        var company = result.Value as CompanyDTO;

        company!.Name.Should().Be(expected_name);
        company.Address.Should().Be(expected_addr);

    }

    [Fact]
    public async Task Delete_Should_ReturnNoContentAsync() {
        // Arrange
        var order_id = Guid.NewGuid();

        var mock = new Mock<IRepository<Company>>();
        mock.Setup(x => x.GetAsync(order_id))
            .ReturnsAsync(() => new(order_id, 0, "", new()));
        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new Delete.Command(context, order_id);
        var handler = new Delete.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        var response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NoContentResult>();

        var result = response as NoContentResult;
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Delete_Should_ReturnNotFound_When_CompanyDoesNotExist() {
        // Arrange
        var order_id = Guid.NewGuid();

        var mock = new Mock<IRepository<Company>>();
        mock.Setup(x => x.GetAsync(order_id))
            .ReturnsAsync(() => null);
        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new Delete.Command(context, order_id);
        var handler = new Delete.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        var response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();

        var result = response as NotFoundObjectResult;
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SetAddress_Should_ReturnCompanyAsync() {
        // Arrange
        var order_id = Guid.NewGuid();
        var expected_addr = new AddressDTO() {
            Line1 = "A",
            Line2 = "B",
            City = "C",
            State = "D",
            Zip = "E"
        };

        var mock = new Mock<IRepository<Company>>();
        mock.Setup(x => x.GetAsync(order_id))
            .ReturnsAsync(() => new(order_id, 0, "", new()));
        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new SetAddress.Command(context, order_id, expected_addr);
        var handler = new SetAddress.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        var response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<OkObjectResult>();

        var result = response as OkObjectResult;
        result.Should().NotBeNull();
        result!.Value.Should().BeOfType<CompanyDTO>();

        var company = result.Value as CompanyDTO;

        company!.Id.Should().Be(order_id);
        company.Address.Should().Be(expected_addr);
    }

    [Fact]
    public async Task SetAddress_ShouldReturnNotFound_WhenCompanyDoesNotExist() {
        // Arrange
        var order_id = Guid.NewGuid();

        var mock = new Mock<IRepository<Company>>();
        mock.Setup(x => x.GetAsync(order_id))
            .ReturnsAsync(() => null);
        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new SetAddress.Command(context, order_id, new());
        var handler = new SetAddress.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        var response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();

        var result = response as NotFoundObjectResult;
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task SetName_Should_ReturnCompanyAsync() {
        // Arrange
        var order_id = Guid.NewGuid();
        var new_name = new NewCompanyName() { Name = "New Name" };

        var mock = new Mock<IRepository<Company>>();
        mock.Setup(x => x.GetAsync(order_id))
            .ReturnsAsync(() => new(order_id, 0, "", new()));
        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new SetName.Command(context, order_id, new_name);
        var handler = new SetName.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        var response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<OkObjectResult>();

        var result = response as OkObjectResult;
        result.Should().NotBeNull();
        result!.Value.Should().BeOfType<CompanyDTO>();

        var company = result.Value as CompanyDTO;

        company!.Id.Should().Be(order_id);
        company.Name.Should().Be(new_name.Name);
    }

    [Fact]
    public async Task SetName_ShouldReturnNotFound_WhenCompanyDoesNotExist() {
        // Arrange
        var order_id = Guid.NewGuid();

        var mock = new Mock<IRepository<Company>>();
        mock.Setup(x => x.GetAsync(order_id))
            .ReturnsAsync(() => null);
        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new SetName.Command(context, order_id, new() { Name = "" });
        var handler = new SetName.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        var response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();

        var result = response as NotFoundObjectResult;
        result.Should().NotBeNull();
    }



}
