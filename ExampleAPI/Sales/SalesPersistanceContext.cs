using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Companies.Domain;
using ExampleAPI.Sales.Orders.Domain;

namespace ExampleAPI.Sales.Data;

public class SalesPersistanceContext : IPersistanceContext {

    public PersistanceSet<Company> Companies { get; init; }
    public PersistanceSet<Order> Orders { get; init; }

    public SalesPersistanceContext(PersistanceSet<Company> companies, PersistanceSet<Order> orders) {
        Companies = companies;
        Orders = orders;
    }

    public async Task SaveChanges() {
        // TODO: use transactions
        await Companies.SaveChanges();
        await Orders.SaveChanges();
    }

}