using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Companies.Domain;

namespace ExampleAPI.Sales.Companies.Data;

public interface ICompanyRepository : IRepository<Company> {

    // Put custom company queries / commands here
    public IReadOnlyCollection<Company> ActiveEntities { get; }

}