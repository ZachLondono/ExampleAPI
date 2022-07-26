using ExampleAPI.Sales.Orders.Commands;
using ExampleAPI.Sales.Orders.DTO;
using ExampleAPI.Sales.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Sales.Orders;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase {

    private readonly ILogger<OrdersController> _logger;
    private readonly ISender _sender;

    public OrdersController(ILogger<OrdersController> logger, ISender sender) {
        _logger = logger;
        _sender = sender;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderDTO))]
    public Task<IActionResult> Create([FromBody] NewOrder newOrder) {
        _logger.LogInformation("Creating new order");
        return _sender.Send(new Create.Command(HttpContext, newOrder));        
    }

    [Route("{orderId}/items")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> AddItem(Guid orderId, [FromBody] NewOrderedItem newItem) {
        _logger.LogInformation("Adding item to order {orderId}", orderId);
        return _sender.Send(new AddItem.Command(HttpContext, orderId, newItem));
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<OrderDTO>))]
    public Task<IActionResult> GetAll() {
        _logger.LogInformation("Getting all orders");
        return _sender.Send(new GetAll.Query(HttpContext));
    }

    [Route("{orderId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> Get(Guid orderId) {
        _logger.LogInformation("Getting order {orderId}", orderId);
        return _sender.Send(new Get.Query(HttpContext, orderId));
    }

    [Route("{orderId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid orderId) {
        _logger.LogInformation("Deleting order {orderId}", orderId);
        return await _sender.Send(new Delete.Command(HttpContext, orderId));
    }

    [Route("{orderId}/items/{itemId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteItem(Guid orderId, Guid itemId) {
        _logger.LogInformation("Deleting ordered item {itemId} from order {orderId}", itemId, orderId);
        return await _sender.Send(new RemoveItem.Command(HttpContext, orderId, itemId));
    }

    [Route("{orderId}/name")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> SetName(Guid orderId, [FromBody] NewOrderName newName) {
        _logger.LogInformation("Updating order name {orderId}", orderId);
        return _sender.Send(new SetName.Command(HttpContext, orderId, newName));
    }

    [Route("{orderId}/items/{itemId}")]
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderedItemDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<IActionResult> AdjustItemQty(Guid orderId, Guid itemId, [FromBody] OrderedItemQtyAdjustment itemAdjustment) {
        _logger.LogInformation("Adjusting item order qty {orderId}", orderId);
        return _sender.Send(new AdjustItemQty.Command(HttpContext, orderId, itemId, itemAdjustment));
    }

}
