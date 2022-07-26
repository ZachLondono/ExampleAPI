using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Companies.Data;
using FluentAssertions;
using MediatR;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace ExampleAPI_UnitTests;

public class CompanyRepositoryTests {

    [Fact]
    public async Task Query_Should_ReturnNull() {

        // Arrange
        const string query = "SELECT companies.id, name, line1, line2, city, state, zip, (SELECT version FROM events WHERE companies.id = streamid ORDER BY version DESC LIMIT 1) FROM companies WHERE companies.id = @Id;";

        Guid id = Guid.NewGuid();

        var publisher = new Mock<IPublisher>().Object;
        var transaction = new Mock<IDbTransaction>().Object;
        var mock = new Mock<IDapperConnection>();

        mock.Setup(c => c.QuerySingleOrDefaultAsync<CompanyData?>(query, new { Id = id }, transaction, null, null))
                    .Returns(Task.FromResult<CompanyData?>(null));


        // Act
        var repo = new CompanyRepository(mock.Object, transaction, publisher);


        // Assert
        var order = await repo.GetAsync(id);

        order.Should().BeNull();

    }

}
