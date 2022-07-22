using ExampleAPI.Orders.Domain;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ExampleAPI_UnitTests;
public class OrderTests {

    [Fact]
    public void Should_AddItemToOrder() {

        // Arrange
        var order = new Order(0, "Test Order", Enumerable.Empty<OrderedItem>());

        // Act 
        var item = order.AddItem("New Item", 5);

        // Assert
        Assert.Contains(order.Items, (i) => i.Equals(item));

    }

    [Fact]
    public void Should_CreateEvent_WhenAddingItem() {

        // Arrange
        var order = new Order(0, "Test Order", Enumerable.Empty<OrderedItem>());

        // Act 
        var item = order.AddItem("New Item", 5);

        // Assert
        Assert.Contains(order.Events, (e) => {
            if (e is Events.ItemAddedEvent itemAdded) {
                return itemAdded.Name == item.Name && itemAdded.Qty == item.Qty && itemAdded.Item.Equals(item);
            }

            return false;
        });

    }

    [Fact]
    public void Should_RemoveItemFromOrder() {

        // Arrange
        var item = new OrderedItem(1, "New Item", 5);
        var items = new List<OrderedItem>() { item };
        var order = new Order(0, "Test Order", items);

        // Act 
        order.RemoveItem(item);

        // Assert
        Assert.DoesNotContain(order.Items, (i) => i.Equals(item));

    }

    [Fact]
    public void Should_CreateEvent_WhenRemovingItemFromOrder() {

        // Arrange
        var item = new OrderedItem(1, "New Item", 5);
        var items = new List<OrderedItem>() { item };
        var order = new Order(0, "Test Order", items);

        // Act 
        order.RemoveItem(item);

        // Assert
        Assert.Contains(order.Events, (e) => {
            if (e is Events.ItemRemovedEvent itemRemoved) {
                return itemRemoved.Item.Equals(item);
            }

            return false;
        });

    }

    [Fact]
    public void Should_ChangeName() {

        // Arrange
        var order = new Order(0, "Test Order", Enumerable.Empty<OrderedItem>());

        // Act
        string newName = "Changed Name";
        order.SetName(newName);

        // Assert
        Assert.Equal(newName, order.Name);

    }

    [Fact]
    public void Should_CreateEvent_WhenChangingName() {

        // Arrange
        var order = new Order(0, "Test Order", Enumerable.Empty<OrderedItem>());

        // Act
        string newName = "Changed Name";
        order.SetName(newName);

        // Assert
        Assert.Contains(order.Events, (e) => {
            if (e is Events.OrderNameChangedEvent nameChanged) {
                return nameChanged.Name == order.Name;
            }

            return false;
        });

    }



}