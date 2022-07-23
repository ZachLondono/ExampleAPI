using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Orders.Data;
using MediatR;
using System;
using System.Data;
using System.Text.Json;
using static ExampleAPI.Orders.Domain.Events;

namespace ExampleAPI.Orders.Domain;

public class OrderEventHandler<TNotification> : INotificationHandler<TNotification> where TNotification : OrderEvent {

    private readonly IDbConnection _connection;
    private readonly ILogger<OrderEventHandler<TNotification>> _logger;

    public OrderEventHandler(NpgsqlOrderConnectionFactory factory, ILogger<OrderEventHandler<TNotification>> logger) {
        _connection = factory.CreateConnection();
        _logger = logger;
    }

    public async Task Handle(TNotification notification, CancellationToken cancellationToken) {
        _logger.LogInformation("Storing order event {EventName}", notification.GetType().Name);
        //const string command = "insert into order_events(name, data) values(@Name, @Data)";
        //try { 
        //    await _connection.ExecuteAsync(command, new {
        //        Name = notification.GetType().Name,
        //        Data = new JsonParameter(JsonSerializer.Serialize(notification))
        //    });
        //} catch (Exception e) {
        //    _logger.LogError("Exception occurred while trying to store event {Exception}", e);
        //}
    }

}