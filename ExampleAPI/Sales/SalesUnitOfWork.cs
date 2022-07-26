using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Companies.Data;
using ExampleAPI.Sales.Orders.Data;

namespace ExampleAPI.Sales.Data;

public class SalesUnitOfWork : UnitOfWork {

    private readonly SalesPersistanceContext _context;
    public CompanyRepository Companies { get; init; }
    public OrderRepository Orders { get; init; }

    public SalesUnitOfWork(SalesPersistanceContext context,
                        Func<SalesPersistanceContext, CompanyRepository> createCompanyRepo,
                        Func<SalesPersistanceContext, OrderRepository> createOrderRepo)
                        : base(context) {
        _context = context;

        // Use double dispatch to create repository, avoid creating the dependency
        Companies = createCompanyRepo(_context);
        Orders = createOrderRepo(_context);
    }

}