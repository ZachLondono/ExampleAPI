using MediatR;

namespace ExampleAPI.Common;

public abstract record DomainEvent : INotification {

    public bool IsPublished { get; private set; } = false;

    public void Publish(IPublisher publisher) {
        if (IsPublished) return;
        _ = publisher.Publish(this);
        IsPublished = true;
    }

}
