using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Orders.Commands;
using ExampleAPI.Orders.Data;
using ExampleAPI.Orders.Domain;
using ExampleAPI.Orders.DTO;
using ExampleAPI.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ExampleAPI.Orders;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase {

    private readonly ILogger<OrderController> _logger;
    private readonly ISender _sender;

    public OrderController(ILogger<OrderController> logger, ISender sender) {
        _logger = logger;
        _sender = sender;
    }

    [HttpPost]
    public Task<OrderDTO> Create([FromBody] NewOrder newOrder) {
        _logger.LogInformation("Creating new order");
        return _sender.Send(new Create.Command(newOrder));        
    }

    [Route("GetAllOrders")]
    [HttpGet]
    public Task<IEnumerable<OrderDTO>> GetAll() {
        _logger.LogInformation("Getting all orders");
        return _sender.Send(new GetAll.Query());
    }

    [Route("GetOrder/{orderId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Get(int orderId) {
        _logger.LogInformation("Getting order {orderId}", orderId);
        return _sender.Send(new Get.Query(orderId));
    }

    [Route("DeleteOrder/{orderId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int orderId) {
        _logger.LogInformation("Deleting order {orderId}", orderId);
        await _sender.Send(new Delete.Command(orderId));
        return NoContent();
    }

    [Route("SetName/{orderId}/{newName}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> SetName(int orderId, string newName) {
        _logger.LogInformation("Updating order name {orderId}", orderId);
        return _sender.Send(new SetName.Command(orderId, newName));
    }

    [Route("AddItem/{orderId}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> AddItem(int orderId, [FromBody] NewOrderedItem newItem) {
        _logger.LogInformation("Adding item to order {orderId}", orderId);
        return _sender.Send(new AddItem.Command(orderId, newItem));
    }

    [Route("AdjustItem/{orderId}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderedItemDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> AdjustItemQty(int orderId, [FromBody] OrderedItemQtyAdjustment itemAdjustment) {
        _logger.LogInformation("Adjusting item order qty {orderId}", orderId);
        return _sender.Send(new AdjustItemQty.Command(orderId, itemAdjustment));
    }

}
