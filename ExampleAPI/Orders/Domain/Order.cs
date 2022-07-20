using ExampleAPI.Common;

namespace ExampleAPI.Orders.Domain;

public class Order : Entity {

    public string Name { get; init; }

    private List<OrderedItem> _items { get; init; }
    public IReadOnlyCollection<OrderedItem> Items => _items.AsReadOnly();

    public Order(int id, string name, IEnumerable<OrderedItem> items) : base(id) {
        Name = name;
        _items = new(items);
    }

    public OrderedItem AddItem(string name, int qty) {
        AddEvent(new Events.ItemAddedEvent(name, qty));
        var item = new OrderedItem(-1, name, qty);
        _items.Add(item);
        return item;
    }

    public void RemoveItem(OrderedItem item) {
        AddEvent(new Events.ItemRemovedEvent(item));
        _items.Remove(item);
    }

}