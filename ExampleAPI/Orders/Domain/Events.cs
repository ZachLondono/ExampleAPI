using ExampleAPI.Common;

namespace ExampleAPI.Orders.Domain;

public static class Events {

    public record OrderEvent() : DomainEvent;

    public record ItemAddedEvent(OrderedItem Item, string Name, int Qty) : OrderEvent;

    public record ItemRemovedEvent(OrderedItem Item) : OrderEvent;

    public record ItemQtyAdjustedEvent(OrderedItem Item, int AdjustedQty) : OrderEvent;

    public record OrderNameChangedEvent(string Name) : OrderEvent;

}
