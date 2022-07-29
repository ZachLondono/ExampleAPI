using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Companies.Data;
using ExampleAPI.Sales.Orders.Data;

namespace ExampleAPI.Sales;

public interface ISalesUnitOfWork : IUnitOfWork {

    public IOrderRepository Orders { get; }
    public ICompanyRepository Companies { get; }

}
