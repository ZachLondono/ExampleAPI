using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Orders.Domain;

namespace ExampleAPI.Sales.Orders.Data;

public interface IOrderRepository : IRepository<Order> {

    // Put custom order queries / commands here

}
