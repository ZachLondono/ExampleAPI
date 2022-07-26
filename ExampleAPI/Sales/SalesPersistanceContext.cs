using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Companies.Data;
using ExampleAPI.Sales.Orders.Data;

namespace ExampleAPI.Sales.Data;

public class SalesPersistanceContext : IPersistanceContext, IDisposable {

    public CompanySet Companies { get; init; }
    public OrderSet Orders { get; init; }

    public SalesPersistanceContext(CompanySet companies, OrderSet orders) {
        Companies = companies;
        Orders = orders;
    }

    public async Task SaveChanges() {
        // TODO: use transactions
        await Companies.SaveChanges();
        await Orders.SaveChanges();
    }

    public void Dispose() {
        Orders.Dispose();
        Companies.Dispose();
        GC.SuppressFinalize(this);
    }
}