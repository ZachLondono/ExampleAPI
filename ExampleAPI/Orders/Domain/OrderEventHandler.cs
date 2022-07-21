using MediatR;
using static ExampleAPI.Orders.Domain.Events;

namespace ExampleAPI.Orders.Domain;

public class OrderEventHandler<TNotification> : INotificationHandler<TNotification> where TNotification : OrderEvent {

    private readonly ILogger<OrderEventHandler<TNotification>> _logger;

    public OrderEventHandler(ILogger<OrderEventHandler<TNotification>> logger) {
        _logger = logger;
    }

    public Task Handle(TNotification notification, CancellationToken cancellationToken) {
        _logger.LogInformation("Storing order event {EventName}", notification.GetType().Name);
        return Task.CompletedTask;
    }
}