using ExampleAPI.Sales.Companies.Data;
using ExampleAPI.Sales.Orders.Data;

namespace ExampleAPI.Sales;

public class SalesUnitOfWork : ISalesUnitOfWork {

    public IOrderRepository Orders { get; init; }
    public ICompanyRepository Companies { get; init; }

    public SalesUnitOfWork(IOrderRepository orders, ICompanyRepository companies) {
        Orders = orders;
        Companies = companies;
    }

    public Task CommitAsync() {
        
        // The unit of work should create a transaction and pass it to the repositories to use
        // When save changes is called here, the transaction is commited

        return Task.CompletedTask;
    }

}