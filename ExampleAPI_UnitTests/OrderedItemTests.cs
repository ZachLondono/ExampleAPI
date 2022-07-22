using ExampleAPI.Orders.Domain;
using Xunit;

namespace ExampleAPI_UnitTests;

public class OrderedItemTests {

    [Fact]
    public void Should_AdjustItemQty() {

        // Arrange
        var item = new OrderedItem(1, "Test Item", 1);
        int newQty = 2;

        // Act 
        item.AdjustQty(newQty);

        // Assert
        Assert.Equal(newQty, item.Qty);

    }

    [Fact]
    public void Should_CreateEvent_WhenAdjustingItemQty() {

        // Arrange
        var item = new OrderedItem(1, "Test Item", 1);
        int newQty = 2;

        // Act 
        item.AdjustQty(newQty);

        // Assert
        Assert.Contains(item.Events, (e) => {
            if (e is Events.ItemQtyAdjustedEvent itemAdjusted) {
                return itemAdjusted.Item.Equals(item) && itemAdjusted.Item.Qty == newQty;
            }

            return false;
        });

    }

}
