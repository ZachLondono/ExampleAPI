using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Orders.Domain;
using ExampleAPI.Sales.Data;

namespace ExampleAPI.Sales.Orders.Data;

public class OrderRepository :  IRepository<Order> {

    private readonly SalesPersistanceContext _context;

    public OrderRepository(SalesPersistanceContext context) {
        _context = context;
    }

    public void Add(Order entity) => _context.Orders.Add(entity);

    public Task<Order?> Get(Guid id) => _context.Orders.Get(id);

    public void Remove(Order entity) => _context.Orders.Remove(entity);


}
