namespace ExampleAPI.Common;

public abstract class Entity {

    public int Id { get; init; }

    protected List<DomainEvent> _events = new();
    public IEnumerable<DomainEvent> Events => _events;

    public Entity(int id) => Id = id;

    public void ClearEvents() => _events.Clear();

    protected void AddEvent(DomainEvent domainEvent) => _events.Add(domainEvent);

}
