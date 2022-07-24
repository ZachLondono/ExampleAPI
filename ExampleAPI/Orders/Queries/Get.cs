using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Orders.Data;
using ExampleAPI.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ExampleAPI.Orders.Queries;

public class Get {

    public record Query(HttpContext Context, Guid OrderId) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly IDbConnection _connection;

        public Handler(NpgsqlOrderConnectionFactory factory) {
            _connection = factory.CreateConnection();
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT orders.id, name, (SELECT version FROM events WHERE orders.id = streamid ORDER BY version DESC LIMIT 1) FROM orders WHERE orders.id = @OrderId;";
            OrderDTO? order = await _connection.QuerySingleOrDefaultAsync<OrderDTO>(query, request);

            if (order is null) {
                return new NotFoundObjectResult($"Order with id '{request.OrderId}' not found.");
            }

            const string itemQuery = "SELECT id, name, qty FROM ordereditems WHERE orderid = @OrderId;";
            order.Items = await _connection.QueryAsync<OrderedItemDTO>(itemQuery, request);

            request.Context.Response.Headers.ETag = order.Version.ToString();

            return new OkObjectResult(order);

        }
        
    }
}
