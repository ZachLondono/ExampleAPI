using ExampleAPI.Common;

namespace ExampleAPI.Orders.Domain;

public static class Events {

    public record ItemAddedEvent(OrderedItem Item, string Name, int Qty) : IDomainEvent;

    public record ItemRemovedEvent(OrderedItem Item) : IDomainEvent;

    public record ItemQtyAdjustedEvent(OrderedItem Item, int Qty) : IDomainEvent;

}
