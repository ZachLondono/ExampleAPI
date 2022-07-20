namespace ExampleAPI.Common;

public abstract class Entity {

    public int Id { get; init; }

    protected List<IDomainEvent> _events = new();
    public IEnumerable<IDomainEvent> Events => _events;

    public Entity(int id) => Id = id;

    public void ClearEvents() => _events.Clear();

    protected void AddEvent(IDomainEvent domainEvent) => _events.Add(domainEvent);

}
