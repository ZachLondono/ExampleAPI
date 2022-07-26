using ExampleAPI.Common.Domain;
using ExampleAPI.Common.Data;
using System.Text.Json.Serialization;

namespace ExampleAPI.Sales.Orders.Domain;

public static class Events {

    public record OrderEvent([property: JsonIgnore] Guid OrderId) : DomainEvent(OrderId);

    public record ItemAddedEvent(Guid OrderId, Guid ItemId, string Name, int Qty) : OrderEvent(OrderId);

    public record ItemRemovedEvent(Guid OrderId, Guid ItemId) : OrderEvent(OrderId);

    public record ItemQtyAdjustedEvent(Guid OrderId, Guid ItemId, int AdjustedQty) : OrderEvent(OrderId);

    public record OrderNameChangedEvent(Guid OrderId, string Name) : OrderEvent(OrderId);

    public record OrderCreatedEvent(Guid OrderId, string Name) : OrderEvent(OrderId);

}
