using ExampleAPI.Common;

namespace ExampleAPI.Orders.Domain;

public static class Events {

    public record OrderEvent() : DomainEvent;

    public record ItemAddedEvent(Guid ItemId, string Name, int Qty) : OrderEvent;

    public record ItemRemovedEvent(Guid ItemId) : OrderEvent;

    public record ItemQtyAdjustedEvent(Guid ItemId, int AdjustedQty) : OrderEvent;

    public record OrderNameChangedEvent(string Name) : OrderEvent;

}
