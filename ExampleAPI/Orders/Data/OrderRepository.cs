﻿using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using MediatR;
using System.Data;

namespace ExampleAPI.Orders.Data;

public class OrderRepository :  IRepository<Order> {

    private readonly IDbConnection _connection;
    private readonly IPublisher _publisher;

    public OrderRepository(NpgsqlOrderConnectionFactory factory, IPublisher publisher) {
        _connection = factory.CreateConnection();
        _publisher = publisher;
    }

    public async Task<Order> Create() {

        const string command = "INSERT INTO orders (id, name) values (@Id, @Name);";

        const string defaultName = "New Order";
        Guid newId = Guid.NewGuid();

        await _connection.ExecuteAsync(command, new { Id = newId, Name = defaultName });

        return new(newId, defaultName, Enumerable.Empty<OrderedItem>());

    }

    public async Task<Order?> Get(Guid id) {
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

    private static async Task<IEnumerable<OrderedItem>> GetItemsFromOrderId(IDbConnection connection, Guid orderId, IDbTransaction? transaction = null) {
        const string itemQuery = "SELECT id, name, qty FROM ordereditems WHERE orderid = @OrderId;";

        var itemsData = await connection.QueryAsync<OrderedItemData>(itemQuery, new { OrderId = orderId }, transaction);

        List<OrderedItem> items = new();
        foreach (var item in itemsData) {
            items.Add(new(item.Id, item.Name, item.Qty));
        }

        return items;
    }

    public async Task Remove(Order entity) {
        // PostgreSQL ueses cascading delete to remove ordered items
        const string command = "DELETE FROM orders WHERE id = @OrderId;";
        await _connection.ExecuteAsync(command, new { OrderId = entity.Id });
    }

    public async Task Save(Order entity) {

        _connection.Open();
        var trx = _connection.BeginTransaction();

        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            if (domainEvent is Events.OrderNameChangedEvent nameChanged) {

                const string command = "UPDATE orders SET name = @Name WHERE id = @Id;";

                await _connection.ExecuteAsync(command, new {
                    nameChanged.Name,
                    entity.Id
                }, trx);

            } else if (domainEvent is Events.ItemAddedEvent itemAdded) {

                const string command = "INSERT INTO ordereditems (id, name, qty, orderid) VALUES (@Id, @Name, @Qty, @OrderId);";
                await _connection.ExecuteAsync(command, new {
                    Id = itemAdded.ItemId,
                    itemAdded.Name,
                    itemAdded.Qty,
                    OrderId = entity.Id
                }, trx);

            } else if (domainEvent is Events.ItemRemovedEvent itemRemoved) {

                const string command = "DELETE FROM ordereditems WHERE id = @Id;";
                await _connection.ExecuteAsync(command, new {
                    Id = itemRemoved.ItemId,
                }, trx);

            }

        }

        foreach (var item in entity.Items) {

            await SaveItem(entity, item, _connection, trx);

        }

        trx.Commit();
        _connection.Close();

        entity.PublishEvents(_publisher);
        entity.ClearEvents();
        foreach (var item in entity.Items) {
            item.PublishEvents(_publisher);
            item.ClearEvents();
        }

    }
    
    private static async Task SaveItem(Order order, OrderedItem entity, IDbConnection connection, IDbTransaction trx) {
        
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
