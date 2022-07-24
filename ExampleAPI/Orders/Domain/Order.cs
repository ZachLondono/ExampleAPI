using ExampleAPI.Common;

namespace ExampleAPI.Orders.Domain;

public class Order : Entity {

    public string Name { get; private set; }

    private List<OrderedItem> _items { get; init; }
    public IReadOnlyCollection<OrderedItem> Items => _items.AsReadOnly();

    public Order(Guid id, string name, IEnumerable<OrderedItem> items) : base(id) {
        Name = name;
        _items = new(items);
    }

    /// <summary>
    /// Private constructor is used to create a new order and initilize it to a valid 'default' 
    /// </summary>
    private Order(string name) : this(Guid.NewGuid(), name, Enumerable.Empty<OrderedItem>()) {
        AddEvent(new Events.OrderCreatedEvent(Id, Name));
    }

    public static Order Create(string name) => new(name);

    public void SetName(string name) {
        AddEvent(new Events.OrderNameChangedEvent(Id, name));
        Name = name;
    }

    public OrderedItem AddItem(string name, int qty) {
        var item = OrderedItem.Create(Id, name, qty);
        AddEvent(new Events.ItemAddedEvent(Id, item.Id, name, qty));
        _items.Add(item);
        return item;
    }

    public void RemoveItem(OrderedItem item) {
        AddEvent(new Events.ItemRemovedEvent(Id, item.Id));
        _items.Remove(item);
    }

}