using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using ExampleAPI.Orders.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Orders;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase {

    private readonly ILogger<OrderController> _logger;
    private readonly IRepository<Order> _repository;

    public OrderController(ILogger<OrderController> logger, IRepository<Order> repository) {
        _logger = logger;
        _repository = repository;
    }

    [Route("GetAllOrders")]
    [HttpGet]
    public async Task<IEnumerable<OrderDTO>> GetAll() {
        //TODO: write query specific code for this endpoint, rather than using repository so that data does not need to be mapped twice
        _logger.LogInformation("Getting all orders");

        var orders =  await _repository.GetAll();
        List<OrderDTO> orderDTOs = new List<OrderDTO>();
        foreach (var order in orders) {

            var itemDTOs = new List<OrderedItemDTO>();

            foreach (var item in order.Items) {
                itemDTOs.Add(new() {
                    Id = item.Id,
                    Name = item.Name,
                    Qty = item.Qty
                });
            }

            orderDTOs.Add(new() {
                Id = order.Id,
                Name = order.Name,
                Items = itemDTOs
            });
        }

        return orderDTOs;
    }

    [Route("GetOrder/{orderId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int orderId) {
        //TODO: write query specific code for this endpoint, rather than using repository so that data does not need to be mapped twice
        _logger.LogInformation("Getting order {orderId}", orderId);
        Order? order = await _repository.Get(orderId);

        if (order is null) {
            return NotFound($"Order with id '{orderId}' not found.");
        }

        var items = new List<OrderedItemDTO>();
        foreach (var item in order.Items) {
            items.Add(new() {
                Id = item.Id,
                Name = item.Name,
                Qty = item.Qty
            });
        }

        var dto = new OrderDTO() {
            Id = order.Id,
            Name = order.Name,
            Items = items
        };

        return Ok(dto);
    }

    [Route("DeleteOrder/{orderId}")]
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int orderId) {
        _logger.LogInformation("Deleting order {orderId}", orderId);
        var order = await _repository.Get(orderId);

        if (order is null) {
            return NotFound($"Order with id '{orderId}' not found.");
        }

        await _repository.Remove(order);

        return NoContent();
    }

    [Route("AddItem/{orderId}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(int orderId, [FromBody] NewOrderedItem newItem) {
        _logger.LogInformation("Adding item to order {orderId}", orderId);
        var order =  await _repository.Get(orderId);

        if (order is null) {
            return NotFound($"Order with id '{orderId}' not found.");
        }

        order.AddItem(newItem.Name, newItem.Qty);

        order = await _repository.Save(order);

        var itemDTOs = new List<OrderedItemDTO>();
        foreach (var item in order.Items.Where(i => i.Id > 0)) {
            itemDTOs.Add(new() {
                Id = item.Id,
                Name = item.Name,
                Qty = item.Qty,
            });
        }

        var orderDto = new OrderDTO() {
            Id = order.Id,
            Name = order.Name,
            Items = itemDTOs
        };

        return Ok(orderDto);
    }

    [Route("AdjustItem/{orderId}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderedItemDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdjustItemQty(int orderId, [FromBody] OrderedItemQtyAdjustment itemAdjustment) {
        _logger.LogInformation("Adjusting item order qty {orderId}", orderId);
        var order = await _repository.Get(orderId);

        if (order is null) {
            return NotFound($"Order with id '{orderId}' not found.");
        }

        OrderedItem? item = order.Items.SingleOrDefault(i => i.Id == itemAdjustment.Id);
        if (item is null) {
            return NotFound($"Item with id '{itemAdjustment.Id}' not found.");
        }

        item.AdjustQty(itemAdjustment.NewQty);
        order = await _repository.Save(order);

        item = order.Items.SingleOrDefault(i => i.Id == itemAdjustment.Id);
        if (item is null) {
            return NotFound($"Item with id '{itemAdjustment.Id}' not found.");
        }

        var itemDto = new OrderedItemDTO() {
            Id = item.Id,
            Name = item.Name,
            Qty = item.Qty
        };

        return Ok(itemDto);

    }

}
