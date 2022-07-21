using MediatR;
using System.Text.Json.Serialization;

namespace ExampleAPI.Common;

public abstract class Entity {
    
    public int Id { get; init; }

    protected List<DomainEvent> _events = new();
    [JsonIgnore]
    public IEnumerable<DomainEvent> Events => _events;

    public Entity(int id) => Id = id;

    public void ClearEvents() => _events.Clear();

    protected void AddEvent(DomainEvent domainEvent) => _events.Add(domainEvent);

    public void PublishEvents(IPublisher publisher) {
        foreach (DomainEvent domainEvent in _events) {
            domainEvent.Publish(publisher);
        }
    }

}
