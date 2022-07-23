using ExampleAPI.Common;

namespace ExampleAPI.Orders.Domain;

public class OrderedItem : Entity {

    public int Qty { get; private set; }

    public string Name { get; init; }

    public OrderedItem(Guid id, string name, int qty) : base(id) {
        Qty = qty;
        Name = name;
    }

    public void AdjustQty(int newQty) {
        AddEvent(new Events.ItemQtyAdjustedEvent(Id, newQty));
        Qty = newQty;
    }

}