using Dapper;
using ExampleAPI.Common;
using MediatR;
using System.Data;
using System.Text.Json;
using static ExampleAPI.Companies.Domain.Events;

namespace ExampleAPI.Companies.Domain;

public class CompanyEventHandler<TNotification> : INotificationHandler<TNotification> where TNotification : CompanyEvent {

    private readonly IDbConnection _connection;
    private readonly ILogger<CompanyEventHandler<TNotification>> _logger;

    public CompanyEventHandler(NpgsqlOrderConnectionFactory factory, ILogger<CompanyEventHandler<TNotification>> logger) {
        _connection = factory.CreateConnection();
        _logger = logger;
    }

    public async Task Handle(TNotification notification, CancellationToken cancellationToken) {
        _logger.LogInformation("Storing company event {EventName}", notification.GetType().Name);
        //const string command = "insert into company_events(name, data) values(@Name, @Data)";
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
