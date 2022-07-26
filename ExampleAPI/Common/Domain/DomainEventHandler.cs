using Dapper;
using ExampleAPI.Common.Data;
using MediatR;
using System.Data;
using System.Text.Json;

namespace ExampleAPI.Common.Domain;

public class DomainEventHandler<TNotification> : INotificationHandler<TNotification> where TNotification : DomainEvent { 

    private readonly IDbConnection _connection;
    private readonly ILogger<DomainEventHandler<TNotification>> _logger;

    public DomainEventHandler(NpgsqlOrderConnectionFactory factory, ILogger<DomainEventHandler<TNotification>> logger) {
        _connection = factory.CreateConnection();
        _logger = logger;
    }

    public async Task Handle(TNotification notification, CancellationToken cancellationToken) {
        _logger.LogInformation("Storing domain event {EventName}", notification.GetType().Name);

        const string versionQuery = @"select version from events
                                        where streamid = @StreamId
                                        order by version desc;";

        const string command = @"insert into events(id, streamid, version, correlationid, type, data)
                                values(@Id, @StreamId, @Version, @CorrelationId, @Type, @Data);";
        try {

            int version = await _connection.QueryFirstOrDefaultAsync<int>(versionQuery, new { StreamId = notification.AggregateId });
            version++;

            Guid? correleationId = null;

            await _connection.ExecuteAsync(command, new {
                Id = notification.EventId,
                Type = notification.GetType().Name,
                StreamId = notification.AggregateId,
                Version = version,
                CorrelationId = correleationId,
                Data = new JsonParameter(JsonSerializer.Serialize(notification))
            });

        } catch (Exception e) {
            _logger.LogError("Exception occurred while trying to store event {Exception} {Event}", e, notification);
        }
    }
}
