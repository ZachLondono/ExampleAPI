using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using System.Data;

namespace ExampleAPI.Orders.Data;

public class OrderRepository : IRepository<Order> {

    private readonly NpgsqlOrderConnectionFactory _factory;

    public OrderRepository(NpgsqlOrderConnectionFactory factory) {
        _factory = factory;
    }

    public Task<Order> Create() {
        throw new NotImplementedException();
    }

    public async Task<Order?> Get(int id) {
        var connection = _factory.CreateConnection();

        const string orderQuery = "SELECT id, name FROM orders WHERE id = @Id;";

        var orderData = await connection.QuerySingleOrDefaultAsync<OrderData>(orderQuery, new { Id = id });

        if (orderData == default) {
            return null;
        }

        var items = await GetItemsFromOrderId(connection, id);

        var order =  new Order(orderData.Id, orderData.Name, items);

        return order;
    }

    public async Task<IEnumerable<Order>> GetAll() {

        var connection = _factory.CreateConnection();

        const string query = "SELECT id, name FROM orders;";

        var ordersData = await connection.QueryAsync<OrderData>(query);

        List<Order> orders = new();
        foreach (var orderData in ordersData) {

            var items = await GetItemsFromOrderId(connection, orderData.Id);

            orders.Add(new(orderData.Id, orderData.Name, items));

        }

        return orders;

    }

    public Task<Order> Remove(Order entity) {
        throw new NotImplementedException();
    }

    public Task<Order> Save(Order entity) {
        throw new NotImplementedException();
    }

    private async Task<IEnumerable<OrderedItem>> GetItemsFromOrderId(IDbConnection connection, int orderId, IDbTransaction? transaction = null) {
        const string itemQuery = "SELECT id, name, qty FROM ordereditems WHERE orderid = @OrderId;";

        var itemsData = await connection.QueryAsync<OrderedItemData>(itemQuery, new { OrderId = orderId }, transaction);

        List<OrderedItem> items = new List<OrderedItem>();
        foreach (var item in itemsData) {
            items.Add(new(item.Id, item.Name, item.Qty));
        }

        return items;
    } 

}
