using MediatR;
using System.Text.Json.Serialization;

namespace ExampleAPI.Common;

public abstract record DomainEvent : INotification {

    [JsonIgnore]
    public bool IsPublished { get; private set; } = false;

    public void Publish(IPublisher publisher) {
        if (IsPublished) return;
        _ = publisher.Publish(this);
        IsPublished = true;
    }

}
