using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ExampleAPI.Orders.Queries;

public class GetAll {

    public record Query(HttpContext Context) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly IDbConnection _connection;

        public Handler(NpgsqlOrderConnectionFactory factory) {
            _connection = factory.CreateConnection();
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT orders.id, name, (SELECT version FROM events WHERE orders.id = streamid ORDER BY version DESC LIMIT 1) FROM orders;";
            var orders = await _connection.QueryAsync<OrderDTO>(query);

            foreach (var order in orders) {
                var items = await GetItemsFromOrderId(_connection, order.Id);
                order.Items = items;
            }

            return new OkObjectResult(orders);

        }
        
        private static async Task<IEnumerable<OrderedItemDTO>> GetItemsFromOrderId(IDbConnection connection, Guid orderId, IDbTransaction? transaction = null) {
            const string itemQuery = "SELECT id, name, qty FROM ordereditems WHERE orderid = @OrderId;";
            return await connection.QueryAsync<OrderedItemDTO>(itemQuery, new { OrderId = orderId }, transaction);
        }

    }


}