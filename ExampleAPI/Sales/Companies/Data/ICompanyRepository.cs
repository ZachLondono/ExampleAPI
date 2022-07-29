using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Companies.Domain;
using MediatR;

namespace ExampleAPI.Sales.Companies.Data;

public interface ICompanyRepository : IRepository<Company> {

    // Put custom company queries / commands here
    public Task PublishEvents(IPublisher publisher);

}