using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using System.Data;

namespace ExampleAPI.Orders.Data;

public class OrderRepository :  IRepository<Order> {

    private readonly IDbConnection _connection;

    public OrderRepository(NpgsqlOrderConnectionFactory factory) {
        _connection = factory.CreateConnection();
    }

    public Task<Order> Create() {
        throw new NotImplementedException();
    }

    public async Task<Order?> Get(int id) {
        const string orderQuery = "SELECT id, name FROM orders WHERE id = @Id;";

        var orderData = await _connection.QuerySingleOrDefaultAsync<OrderData>(orderQuery, new { Id = id });

        if (orderData == default) {
            return null;
        }

        var items = await GetItemsFromOrderId(_connection, id);

        var order =  new Order(orderData.Id, orderData.Name, items);

        return order;
    }

    public async Task<IEnumerable<Order>> GetAll() {
        const string query = "SELECT id, name FROM orders;";

        var ordersData = await _connection.QueryAsync<OrderData>(query);

        List<Order> orders = new();
        foreach (var orderData in ordersData) {

            var items = await GetItemsFromOrderId(_connection, orderData.Id);

            orders.Add(new(orderData.Id, orderData.Name, items));

        }

        return orders;

    }

    public async Task Remove(Order entity) {
        // PostgreSQL ueses cascading delete to remove ordered items
        const string command = "DELETE FROM orders WHERE id = @OrderId;";
        await _connection.ExecuteAsync(command, new { OrderId = entity.Id });
    }

    private async Task<IEnumerable<OrderedItem>> GetItemsFromOrderId(IDbConnection connection, int orderId, IDbTransaction? transaction = null) {
        const string itemQuery = "SELECT id, name, qty FROM ordereditems WHERE orderid = @OrderId;";

        var itemsData = await _connection.QueryAsync<OrderedItemData>(itemQuery, new { OrderId = orderId }, transaction);

        List<OrderedItem> items = new List<OrderedItem>();
        foreach (var item in itemsData) {
            items.Add(new(item.Id, item.Name, item.Qty));
        }

        return items;
    }

    public async Task<Order> Save(Order entity) {

        _connection.Open();
        var trx = _connection.BeginTransaction();

        List<OrderedItem> items = new(entity.Items);

        foreach (var domainEvent in entity.Events) {

            if (domainEvent is Events.ItemAddedEvent itemAdded) {

                const string command = "INSERT INTO ordereditems (name, qty, orderid) VALUES (@Name, @Qty, @OrderId) RETURNING id;";
                int newItemId = await _connection.QuerySingleAsync<int>(command, new {
                    Name = itemAdded.Name,
                    Qty = itemAdded.Qty,
                    OrderId = entity.Id
                }, trx);

                items.Remove(itemAdded.Item);
                items.Add(new(newItemId, itemAdded.Name, itemAdded.Qty));


            } else if (domainEvent is Events.ItemRemovedEvent itemRemoved) {

                items.Remove(itemRemoved.Item);

                // If the item has not yet been persisted, there is no need to try to delete it
                if (itemRemoved.Item.Id <= 0) continue;

                const string command = "DELETE FROM ordereditems WHERE id = @Id;";
                await _connection.QuerySingleAsync<int>(command, new {
                    Id = itemRemoved.Item.Id,
                }, trx);

            }

        }

        foreach (var item in entity.Items) {

            if (item.Id <= 0) continue;
            await SaveItem(entity, item, _connection, trx);

        }

        trx.Commit();
        _connection.Close();

        return new Order(entity.Id, entity.Name, items);

    }
    
    private static async Task SaveItem(Order order, OrderedItem entity, IDbConnection connection, IDbTransaction trx) {
        
        foreach (var domainEvent in entity.Events) {

            if (domainEvent is Events.ItemQtyAdjustedEvent itemAdjustment) {

                const string command = "UPDATE ordereditems SET qty = @Qty WHERE id = @Id;";

                await connection.ExecuteAsync(command, new {
                    Id = itemAdjustment.Item.Id,
                    Qty = itemAdjustment.Qty
                }, trx);

            }

        }

    }

}
