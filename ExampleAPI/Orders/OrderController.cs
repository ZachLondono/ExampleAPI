using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Orders.Data;
using ExampleAPI.Orders.Domain;
using ExampleAPI.Orders.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ExampleAPI.Orders;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase {

    private readonly ILogger<OrderController> _logger;
    private readonly IRepository<Order> _repository;
    private readonly NpgsqlOrderConnectionFactory _factory;

    public OrderController(ILogger<OrderController> logger, IRepository<Order> repository, NpgsqlOrderConnectionFactory factory) {
        _logger = logger;
        _repository = repository;
        _factory = factory;
    }

    [HttpPost]
    public async Task<OrderDTO> Create([FromBody] NewOrder newOrder) {
        _logger.LogInformation("Creating new order");

        var order = await _repository.Create();
        order.SetName(newOrder.Name);
        foreach (var item in newOrder.NewItems) {
            order.AddItem(item.Name, item.Qty);
        }

        order = await _repository.Save(order);

        var itemDTOs = new List<OrderedItemDTO>();

        foreach (var item in order.Items) {
            itemDTOs.Add(new() {
                Id = item.Id,
                Name = item.Name,
                Qty = item.Qty
            });
        }

        return new OrderDTO() {
            Id = order.Id,
            Name = order.Name,
            Items = itemDTOs
        };
    }

    [Route("GetAllOrders")]
    [HttpGet]
    public async Task<IEnumerable<OrderDTO>> GetAll() {
        _logger.LogInformation("Getting all orders");

        var connection = _factory.CreateConnection();
        const string query = "SELECT id, name FROM orders;";
        var orders = await connection.QueryAsync<OrderDTO>(query);

        foreach (var order in orders) {
            var items = await GetItemsFromOrderId(connection, order.Id);
            order.Items = items;
        }

        return orders;
    }

    [Route("GetOrder/{orderId}")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(int orderId) {
        _logger.LogInformation("Getting order {orderId}", orderId);
        
        var connection = _factory.CreateConnection();
        const string query = "SELECT id, name FROM orders;";
        OrderDTO? order = await connection.QuerySingleOrDefaultAsync<OrderDTO>(query);

        if (order is null) {
            return NotFound($"Order with id '{orderId}' not found.");
        }

        order.Items = await GetItemsFromOrderId(connection, order.Id);

        return Ok(order);
    }

    private static async Task<IEnumerable<OrderedItemDTO>> GetItemsFromOrderId(IDbConnection connection, int orderId, IDbTransaction? transaction = null) {
        const string itemQuery = "SELECT id, name, qty FROM ordereditems WHERE orderid = @OrderId;";
        return await connection.QueryAsync<OrderedItemDTO>(itemQuery, new { OrderId = orderId }, transaction);
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

    [Route("SetName/{orderId}/{newName}")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItem(int orderId, string newName) {
        _logger.LogInformation("Updating order name {orderId}", orderId);
        var order = await _repository.Get(orderId);

        if (order is null) {
            return NotFound($"Order with id '{orderId}' not found.");
        }

        order.SetName(newName);

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
