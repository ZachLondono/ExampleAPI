﻿using ExampleAPI.Common;

namespace ExampleAPI.Orders.Domain;

public class OrderedItem : Entity {

    public Guid OrderId { get; init; }

    public int Qty { get; private set; }

    public string Name { get; init; }

    public OrderedItem(Guid id, Guid orderId, string name, int qty) : base(id) {
        OrderId = orderId;
        Qty = qty;
        Name = name;
    }

    public void AdjustQty(int newQty) {
        AddEvent(new Events.ItemQtyAdjustedEvent(OrderId, Id, newQty));
        Qty = newQty;
    }

}