using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Orders.Commands;
using ExampleAPI.Sales.Orders.Domain;
using ExampleAPI.Sales.Orders.DTO;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
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
        var expected_order = new Order(expected_id, 0, expected_name, Enumerable.Empty<OrderedItem>());
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

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new AddItem.Command(context, expected_id, to_add);
        var handler = new AddItem.Handler(repo);
        var token = new CancellationTokenSource().Token;
        
        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert

        response.Should().BeOfType<CreatedResult>();

        var okResponse = response as CreatedResult;
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

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new AddItem.Command(context, id, new());
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
            .ReturnsAsync(() => new(order_id, 0,"Example Order", new List<OrderedItem>() {
                new(item_id, 0, order_id, "Example Item", original_qty)
            }));

        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new AdjustItemQty.Command(context, order_id, item_id, new() { NewQty = new_qty });
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

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new AdjustItemQty.Command(context, id, Guid.NewGuid(), new() { NewQty = 10 });
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
            .ReturnsAsync(() => new(order_id, 0, "Example Order", Enumerable.Empty<OrderedItem>()));

        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new AdjustItemQty.Command(context, order_id, item_id, new() { NewQty = 10 });
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

        var original_name = "Original name";
        var new_name = new NewOrderName() { Name = "New_name" };

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id))
            .ReturnsAsync(() => new(order_id, 0, original_name, Enumerable.Empty<OrderedItem>()));

        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new SetName.Command(context, order_id, new_name);
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
        order_response.Name.Should().Be(new_name.Name);

    }

    [Fact]
    public async Task SetName_Should_ReturnNotFound_WhenSettingNameOfOrderThatDoesNotExistAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();

        var new_name = new NewOrderName() { Name = "New_name" };

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id))
            .ReturnsAsync(() => null);

        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new SetName.Command(context, order_id, new_name);
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
            .ReturnsAsync(() => new(order_id, 0, "", Enumerable.Empty<OrderedItem>()));

        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new Delete.Command(context, order_id);
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

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new Delete.Command(context, order_id);
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
        string expected_name = "Test Order";

        var mock = new Mock<IRepository<Order>>();
        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new Create.Command(context, new() { Name = expected_name, NewItems = Enumerable.Empty<NewOrderedItem>()});
        var handler = new Create.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<CreatedResult>();
        var okResponse = response as CreatedResult;

        okResponse!.Value.Should().BeOfType<OrderDTO>();
        var order_response = okResponse.Value as OrderDTO;
        order_response!.Name.Should().Be(expected_name);

    }

    [Fact]
    public async Task RemoveItem_Should_ReturnNoContent_When_RemovingItemFromOrderAsync() {

        // Arrange
        Guid order_id = Guid.NewGuid();
        Guid item_id = Guid.NewGuid();

        var mock = new Mock<IRepository<Order>>();
        mock.Setup(x => x.Get(order_id))
            .ReturnsAsync(() => new(order_id, 0, "Example Order", new List<OrderedItem>() {
                new(item_id, 0, order_id, "Example Item", 5)
            }));

        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new RemoveItem.Command(context, order_id, item_id);
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

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new RemoveItem.Command(context, order_id, item_id);
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
            .ReturnsAsync(() => new(order_id, 0, "Example Order", Enumerable.Empty<OrderedItem>()));

        var repo = mock.Object;

        var httpmock = new Mock<HttpContext>();
        var context = httpmock.Object;

        var request = new RemoveItem.Command(context, order_id, item_id);
        var handler = new RemoveItem.Handler(repo);
        var token = new CancellationTokenSource().Token;

        // Act
        IActionResult response = await handler.Handle(request, token);

        // Assert
        response.Should().BeOfType<NotFoundObjectResult>();
        var okResponse = response as NotFoundObjectResult;

    }

}
