using Dapper;
using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Orders.Domain;
using MediatR;
using System.Data;

namespace ExampleAPI.Sales.Orders.Data;

public class OrderSet : PersistanceSet<Order> {

    private readonly IDbConnection _connection;
    private readonly IPublisher _publisher;

    public OrderSet(NpgsqlOrderConnectionFactory factory, IPublisher publisher) {
        _connection = factory.CreateConnection();
        _publisher = publisher;
    }

    public override async Task<Order?> Get(Guid id) {

        const string orderQuery = "SELECT orders.id, name, (SELECT version FROM events WHERE orders.id = streamid ORDER BY version DESC LIMIT 1) FROM orders WHERE orders.id = @Id;";

        var orderData = await _connection.QuerySingleOrDefaultAsync<OrderData>(orderQuery, new { Id = id });

        if (orderData == default) {
            return null;
        }

        var items = await GetItemsFromOrderId(_connection, id);

        var order = new Order(orderData.Id, orderData.Version, orderData.Name, items);

        Entities.Add(order);

        return order;

    }

    private static async Task<IEnumerable<OrderedItem>> GetItemsFromOrderId(IDbConnection connection, Guid orderId, IDbTransaction? transaction = null) {
        const string itemQuery = "SELECT id, name, qty FROM ordereditems WHERE orderid = @OrderId;";

        var itemsData = await connection.QueryAsync<OrderedItemData>(itemQuery, new { OrderId = orderId }, transaction);

        List<OrderedItem> items = new();
        foreach (var item in itemsData) {
            items.Add(new(item.Id, 0, orderId, item.Name, item.Qty));
        }

        return items;
    }

    public override async Task SaveChanges() {

        _connection.Open();
        var trx = _connection.BeginTransaction();

        foreach (var entity in Entities) {
            
            await SaveEntity(entity, _connection, trx);

        }

        foreach (var entity in RemovedEntities) {

            const string command = "DELETE FROM orders WHERE id = @OrderId;";
            await _connection.ExecuteAsync(command, new { OrderId = entity.Id }, trx);

        }

        trx.Commit();
        _connection.Close();

        foreach (var entity in Entities) {
            await entity.PublishEvents(_publisher);
            entity.ClearEvents();
            foreach (var item in entity.Items) {
                int eventsPublished = await item.PublishEvents(_publisher);
                entity.IncrementVersion(eventsPublished);
                item.ClearEvents();
            }
        }

    }

    private static async Task SaveEntity(Order entity, IDbConnection connection, IDbTransaction trx) {

        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            if (domainEvent is Events.OrderCreatedEvent created) {

                const string command = "INSERT INTO orders (id, name) values (@Id, @Name);";
                await connection.ExecuteAsync(command, new { entity.Id, created.Name }, trx);

            } if (domainEvent is Events.OrderNameChangedEvent nameChanged) {

                const string command = "UPDATE orders SET name = @Name WHERE id = @Id;";

                await connection.ExecuteAsync(command, new {
                    nameChanged.Name,
                    entity.Id
                }, trx);

            } else if (domainEvent is Events.ItemAddedEvent itemAdded) {

                await InsertItem(entity, itemAdded.ItemId, itemAdded.Name, itemAdded.Qty, connection, trx);

            } else if (domainEvent is Events.ItemRemovedEvent itemRemoved) {

                const string command = "DELETE FROM ordereditems WHERE id = @Id;";
                await connection.ExecuteAsync(command, new {
                    Id = itemRemoved.ItemId,
                }, trx);

            }

        }

        foreach (var item in entity.Items) {
            await SaveItem(item, connection, trx);
        }

    }

    private static async Task InsertItem(Order entity, Guid itemId, string itemName, int itemQty, IDbConnection connection, IDbTransaction trx) {
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

    public override void Dispose() {
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }

}
