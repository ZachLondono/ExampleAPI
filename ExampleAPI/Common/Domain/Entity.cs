using MediatR;
using System.Text.Json.Serialization;

namespace ExampleAPI.Common.Domain;

public abstract class Entity {
    
    public Guid Id { get; init; }

    // TODO: Version should only be a property on Aggregate Roots, not on any entity
    public int Version { get; private set; }

    protected List<DomainEvent> _events = new();

    [JsonIgnore]
    public IEnumerable<DomainEvent> Events => _events;

    public Entity(Guid id, int version) {
        Id = id;
        Version = version;
    }

    public void ClearEvents() => _events.Clear();

    protected void AddEvent(DomainEvent domainEvent) => _events.Add(domainEvent);

    public async Task<int> PublishEvents(IPublisher publisher) {
        int publishedCount = 0;
        foreach (DomainEvent domainEvent in _events) {
            if (await domainEvent.Publish(publisher)) { 
                publishedCount++;
                Version++;
            }
        }
        return publishedCount;
    }

    public void IncrementVersion(int count) {
        Version += count;
    }

}
