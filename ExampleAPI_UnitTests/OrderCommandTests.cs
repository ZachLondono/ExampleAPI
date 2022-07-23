using ExampleAPI.Common;
using ExampleAPI.Orders.Commands;
using ExampleAPI.Orders.Domain;
using ExampleAPI.Orders.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace ExampleAPI_UnitTests;

public class OrderCommandTests {

    [Fact]
    public async Task AddItem_Should_AddItemToOrderAsync() {

        // Arrange
        var expected_id = Guid.NewGuid();
        var expected_name = "Order Name";
        var expected_order = new Order(expected_id, expected_name, Enumerable.Empty<OrderedItem>());
        var expected_item_name = "Item Name";
        var expected_qty = 5;
        var to_add = new NewOrderedItem() {
            Name = expected_item_name,
            Qty = expected_qty
        };

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(expected_id))
            .ReturnsAsync(() => expected_order);

        var repo = mock.Object;

        var request = new AddItem.Command(expected_id, to_add);
        var handler = new AddItem.Handler(repo);
        var token = new CancellationTokenSource().Token;
        
        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert

        response.Should().BeOfType<OkObjectResult>();

        var okResponse = response as OkObjectResult;
        okResponse.Should().NotBeNull();

        okResponse!.Value.Should().BeOfType<OrderDTO>();
        var actual = okResponse.Value as OrderDTO;

        actual!.Name.Should().Be(expected_name);
        actual.Id.Should().Be(expected_id);
        actual.Items.Should().HaveCount(1);
        actual.Items.Should().OnlyContain(i => i.Name.Equals(expected_item_name) && i.Qty.Equals(expected_qty));

    }

    [Fact]
    public async Task AddItem_Should_ReturnNotFound_WhenAddingItemToOrderThatDoesNotExistAsync() {

        // Arrange
        Guid id = Guid.NewGuid();

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(id))
            .ReturnsAsync(() => null);

        var repo = mock.Object;

        var request = new AddItem.Command(id, new());
        var handler = new AddItem.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();

        var notFoundResponse = response as NotFoundObjectResult;
        notFoundResponse.Should().NotBeNull();

    }

    [Fact]
    public async Task AdjustItemQty_Should_AdjustItemQtyAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();
        Guid item_id = Guid.NewGuid();

        int original_qty = 5;
        int new_qty = 10;

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id))
            .ReturnsAsync(() => new(order_id, "Example Order", new List<OrderedItem>() {
                new(item_id,order_id, "Example Item", original_qty)
            }));

        var repo = mock.Object;

        var request = new AdjustItemQty.Command(order_id, new() { Id = item_id, NewQty = new_qty });
        var handler = new AdjustItemQty.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResponse = response as OkObjectResult;

        okResponse!.Value.Should().BeOfType<OrderedItemDTO>();
        var item_response = okResponse.Value as OrderedItemDTO;

        item_response!.Id.Should().Be(item_id);
        item_response.Qty.Should().Be(new_qty);

    }

    [Fact]
    public async Task AdjustItemQty_Should_ReturnNotFound_WhenAdjusingItemInOrderThatDoesNotExistAsync() {
        
        // Arrange
        Guid id = Guid.NewGuid();

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(id))
            .ReturnsAsync(() => null);

        var repo = mock.Object;

        var request = new AdjustItemQty.Command(id, new());
        var handler = new AdjustItemQty.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();

        var notFoundResponse = response as NotFoundObjectResult;
        notFoundResponse.Should().NotBeNull();

    }

    [Fact]
    public async Task AdjustItemQty_Should_ReturnNotFound_WhenAdjusingItemThatDoesNotExistAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();
        Guid item_id = Guid.NewGuid();

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id))
            .ReturnsAsync(() => new(order_id, "Example Order", Enumerable.Empty<OrderedItem>()));

        var repo = mock.Object;

        var request = new AdjustItemQty.Command(order_id, new());
        var handler = new AdjustItemQty.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();

        var notFoundResponse = response as NotFoundObjectResult;
        notFoundResponse.Should().NotBeNull();

    }

    [Fact]
    public async Task SetName_Should_SetOrderNameAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();

        string original_name = "Original name";
        string new_name = "New_name";

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id))
            .ReturnsAsync(() => new(order_id, original_name, Enumerable.Empty<OrderedItem>()));

        var repo = mock.Object;

        var request = new SetName.Command(order_id, new_name);
        var handler = new SetName.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResponse = response as OkObjectResult;

        okResponse!.Value.Should().BeOfType<OrderDTO>();
        var order_response = okResponse.Value as OrderDTO;

        order_response!.Id.Should().Be(order_id);
        order_response.Name.Should().Be(new_name);

    }

    [Fact]
    public async Task SetName_Should_ReturnNotFound_WhenSettingNameOfOrderThatDoesNotExistAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();

        string new_name = "New_name";

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id))
            .ReturnsAsync(() => null);

        var repo = mock.Object;

        var request = new SetName.Command(order_id, new_name);
        var handler = new SetName.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NotFoundResult>();

        var notFoundResponse = response as NotFoundResult;
        notFoundResponse.Should().NotBeNull();

    }

    [Fact]
    public async Task Delete_Should_ReturnNoCountent_WhenDeletingOrderAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();
        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id))
            .ReturnsAsync(() => new(order_id, "", Enumerable.Empty<OrderedItem>()));

        var repo = mock.Object;

        var request = new Delete.Command(order_id);
        var handler = new Delete.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NoContentResult>();
        var okResponse = response as NoContentResult;

    }

    [Fact]
    public async Task Delete_Should_ReturnNotFound_WhenDeletingOrderThatDoesNotExistAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();
        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id)).ReturnsAsync(() => null);

        var repo = mock.Object;

        var request = new Delete.Command(order_id);
        var handler = new Delete.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();

        var notFoundResponse = response as NotFoundObjectResult;
        notFoundResponse.Should().NotBeNull();

    }

    [Fact]
    public async Task Create_Should_ReturnOrder_WhenCreatingOrderAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();
        string expected_name = "Test Order";

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Create())
            .ReturnsAsync(() => new(order_id, "", Enumerable.Empty<OrderedItem>()));

        var repo = mock.Object;

        var request = new Create.Command(new() { Name = expected_name, NewItems = Enumerable.Empty<NewOrderedItem>()});
        var handler = new Create.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<OkObjectResult>();
        var okResponse = response as OkObjectResult;

        okResponse!.Value.Should().BeOfType<OrderDTO>();
        var order_response = okResponse.Value as OrderDTO;

        order_response!.Id.Should().Be(order_id);
        order_response.Name.Should().Be(expected_name);

    }

    [Fact]
    public async Task RemoveItem_Should_ReturnNoContent_When_RemovingItemFromOrderAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();
        Guid item_id = Guid.NewGuid();

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id))
            .ReturnsAsync(() => new(order_id, "Example Order", new List<OrderedItem>() {
                new(item_id, order_id, "Example Item", 5)
            }));

        var repo = mock.Object;

        var request = new RemoveItem.Command(order_id, item_id);
        var handler = new RemoveItem.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NoContentResult>();
        var okResponse = response as NoContentResult;

    }

    [Fact]
    public async Task RemoveItem_Should_ReturnNotFound_When_RemovingItemFromOrderThatDoesNotExistAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();
        Guid item_id = Guid.NewGuid();

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id))
            .ReturnsAsync(() => null);

        var repo = mock.Object;

        var request = new RemoveItem.Command(order_id, item_id);
        var handler = new RemoveItem.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();
        var okResponse = response as NotFoundObjectResult;

    }

    [Fact]
    public async Task RemoveItem_Should_ReturnNotFound_When_RemovingItemThatDoesNotExistAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();
        Guid item_id = Guid.NewGuid();

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id))
            .ReturnsAsync(() => new(order_id, "Example Order", Enumerable.Empty<OrderedItem>()));

        var repo = mock.Object;

        var request = new RemoveItem.Command(order_id, item_id);
        var handler = new RemoveItem.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();
        var okResponse = response as NotFoundObjectResult;

    }

}
