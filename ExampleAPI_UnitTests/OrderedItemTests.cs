using ExampleAPI.Orders.Domain;
using FluentAssertions;
using System;
using Xunit;

namespace ExampleAPI_UnitTests;

public class OrderedItemTests {

    [Fact]
    public void Should_CreateNewOrderedItem() {

        // Arrange
        Guid orderId = Guid.NewGuid();
        string newname = "New Name";
        int qty = 5;

        // Act
        var item = OrderedItem.Create(orderId, newname, qty);

        // Assert
        item.OrderId.Should().Be(orderId);
        item.Name.Should().Be(newname);
        item.Qty.Should().Be(qty);

    }

    [Fact]
    public void Should_AdjustItemQty() {

        // Arrange
        var item = new OrderedItem(Guid.NewGuid(), Guid.NewGuid(), "Test Item", 1);
        int newQty = 2;

        // Act 
        item.AdjustQty(newQty);

        // Assert
        Assert.Equal(newQty, item.Qty);

    }

    [Fact]
    public void Should_CreateEvent_WhenAdjustingItemQty() {

        // Arrange
        var item = new OrderedItem(Guid.NewGuid(), Guid.NewGuid(), "Test Item", 1);
        int newQty = 2;

        // Act 
        item.AdjustQty(newQty);

        // Assert
        Assert.Contains(item.Events, (e) => {
            if (e is Events.ItemQtyAdjustedEvent itemAdjusted) {
                return itemAdjusted.ItemId.Equals(item.Id) && itemAdjusted.AdjustedQty== newQty;
            }

            return false;
        });

    }

}
