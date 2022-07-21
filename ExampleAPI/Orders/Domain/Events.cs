using ExampleAPI.Common;

namespace ExampleAPI.Orders.Domain;

public static class Events {

    public record ItemAddedEvent(OrderedItem Item, string Name, int Qty) : DomainEvent;

    public record ItemRemovedEvent(OrderedItem Item) : DomainEvent;

    public record ItemQtyAdjustedEvent(OrderedItem Item, int Qty) : DomainEvent;

    public record OrderNameChangedEvent(string name) : DomainEvent;

}
