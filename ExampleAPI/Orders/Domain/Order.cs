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

    public void SetName(string name) {
        AddEvent(new Events.OrderNameChangedEvent(name));
        Name = name;
    }

    public OrderedItem AddItem(string name, int qty) {
        var item = new OrderedItem(Guid.NewGuid(), name, qty);
        AddEvent(new Events.ItemAddedEvent(item.Id, name, qty));
        _items.Add(item);
        return item;
    }

    public void RemoveItem(OrderedItem item) {
        AddEvent(new Events.ItemRemovedEvent(item.Id));
        _items.Remove(item);
    }

}