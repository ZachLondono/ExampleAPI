using MediatR;
using System.Text.Json.Serialization;

namespace ExampleAPI.Common;

public abstract class Entity {
    
    public Guid Id { get; init; }

    public int Version { get; init; }

    protected List<DomainEvent> _events = new();

    [JsonIgnore]
    public IEnumerable<DomainEvent> Events => _events;

    public Entity(Guid id, int version) {
        Id = id;
        Version = version;
    }

    public void ClearEvents() => _events.Clear();

    protected void AddEvent(DomainEvent domainEvent) => _events.Add(domainEvent);

    public async Task PublishEvents(IPublisher publisher) {
        foreach (DomainEvent domainEvent in _events) {
            await domainEvent.Publish(publisher);
        }
    }

}
