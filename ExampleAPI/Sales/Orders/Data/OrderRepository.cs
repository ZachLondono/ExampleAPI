using Dapper;
using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Orders.Domain;
using MediatR;
using System.Data;

namespace ExampleAPI.Sales.Orders.Data;

public class OrderRepository :  IOrderRepository {

    private readonly IDbConnection _connection;
    private readonly IDbTransaction _transaction;
    private readonly IPublisher _publisher;

    public OrderRepository(IDbConnection connection, IDbTransaction transaction, IPublisher publisher) {
        _connection = connection;
        _transaction = transaction;
        _publisher = publisher;
    }

    public async Task AddAsync(Order entity) {

        const string command = "INSERT INTO orders (id, name) values (@Id, @Name);";

        await _connection.ExecuteAsync(command, new { entity.Id, entity.Name }, _transaction);
        foreach (var item in entity.Items) {
            await InsertItem(entity, item.Id, item.Name, item.Qty, _connection, _transaction);
        }

        await entity.PublishEvents(_publisher);

    }

    public async Task<Order?> GetAsync(Guid id) {
        const string orderQuery = "SELECT orders.id, name, (SELECT version FROM events WHERE orders.id = streamid ORDER BY version DESC LIMIT 1) FROM orders WHERE orders.id = @Id;";

        var orderData = await _connection.QuerySingleOrDefaultAsync<OrderData>(orderQuery, new { Id = id }, _transaction);

        if (orderData == default) {
            return null;
        }

        var items = await GetItemsFromOrderId(_connection, id, _transaction);

        var order =  new Order(orderData.Id, orderData.Version, orderData.Name, items);

        return order;
    }

    public async Task<IEnumerable<Order>> GetAllAsync() {
        const string query = "SELECT orders.id, name, (SELECT version FROM events WHERE orders.id = streamid ORDER BY version DESC LIMIT 1) FROM orders;";

        var ordersData = await _connection.QueryAsync<OrderData>(query, _transaction);

        List<Order> orders = new();
        foreach (var orderData in ordersData) {

            var items = await GetItemsFromOrderId(_connection, orderData.Id, _transaction);

            orders.Add(new(orderData.Id, orderData.Version, orderData.Name, items));

        }

        return orders;
    }

    private static async Task<IEnumerable<OrderedItem>> GetItemsFromOrderId(IDbConnection connection, Guid orderId, IDbTransaction transaction) {
        const string itemQuery = "SELECT id, name, qty FROM ordereditems WHERE orderid = @OrderId;";

        var itemsData = await connection.QueryAsync<OrderedItemData>(itemQuery, new { OrderId = orderId }, transaction);

        List<OrderedItem> items = new();
        foreach (var item in itemsData) {
            items.Add(new(item.Id, 0, orderId, item.Name, item.Qty));
        }

        return items;
    }

    public async Task RemoveAsync(Order entity) {
        // PostgreSQL ueses cascading delete to remove ordered items
        const string command = "DELETE FROM orders WHERE id = @OrderId;";
        await _connection.ExecuteAsync(command, new { OrderId = entity.Id }, _transaction);
    }

    public async Task UpdateAsync(Order entity) {

        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            if (domainEvent is Events.OrderNameChangedEvent nameChanged) {

                const string command = "UPDATE orders SET name = @Name WHERE id = @Id;";

                await _connection.ExecuteAsync(command, new {
                    nameChanged.Name,
                    entity.Id
                }, _transaction);

            } else if (domainEvent is Events.ItemAddedEvent itemAdded) {
                
                await InsertItem(entity, itemAdded.ItemId, itemAdded.Name, itemAdded.Qty, _connection, _transaction);

            } else if (domainEvent is Events.ItemRemovedEvent itemRemoved) {

                const string command = "DELETE FROM ordereditems WHERE id = @Id;";
                await _connection.ExecuteAsync(command, new {
                    Id = itemRemoved.ItemId,
                }, _transaction);

            }

        }

        foreach (var item in entity.Items) {
            await SaveItem(item, _connection, _transaction);
        }

        await entity.PublishEvents(_publisher);
        entity.ClearEvents();
        foreach (var item in entity.Items) {
            int eventsPublished = await item.PublishEvents(_publisher);
            entity.IncrementVersion(eventsPublished);
            item.ClearEvents();
        }

    }

    private async static Task InsertItem(Order entity, Guid itemId, string itemName, int itemQty, IDbConnection connection, IDbTransaction trx) {
        const string command = "INSERT INTO ordereditems (id, name, qty, orderid) VALUES (@Id, @Name, @Qty, @OrderId);";
        await connection.ExecuteAsync(command, new {
            Id = itemId,
            Name = itemName,
            Qty = itemQty,
            OrderId = entity.Id
        }, trx);
    }

    private static async Task SaveItem(OrderedItem entity, IDbConnection connection, IDbTransaction trx) {
        
        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            if (domainEvent is Events.ItemQtyAdjustedEvent itemAdjustment) {

                const string command = "UPDATE ordereditems SET qty = @Qty WHERE id = @Id;";

                await connection.ExecuteAsync(command, new {
                    Id = itemAdjustment.ItemId,
                    Qty = itemAdjustment.AdjustedQty
                }, trx);

            }

        }

    }

}
