using Dapper;
using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Orders.Data;
using ExampleAPI.Sales.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ExampleAPI.Sales.Orders.Queries;

public class Get {

    public record Query(HttpContext Context, Guid OrderId) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly NpgsqlSalesConnectionFactory _factory;

        public Handler(NpgsqlSalesConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            using var connection = _factory.CreateConnection();

            const string query = "SELECT orders.id, name, (SELECT version FROM events WHERE orders.id = streamid ORDER BY version DESC LIMIT 1) FROM orders WHERE orders.id = @OrderId;";
            OrderDTO? order = await connection.QuerySingleOrDefaultAsync<OrderDTO>(query, request);

            if (order is null) {
                return new NotFoundObjectResult($"Order with id '{request.OrderId}' not found.");
            }

            const string itemQuery = "SELECT id, name, qty FROM ordereditems WHERE orderid = @OrderId;";
            order.Items = await connection.QueryAsync<OrderedItemDTO>(itemQuery, request);

            try { 
                request.Context.Response.Headers.ETag = order.Version.ToString();
            } catch {
                // log that header could not be set
            }

            return new OkObjectResult(order);

        }
        
    }
}
