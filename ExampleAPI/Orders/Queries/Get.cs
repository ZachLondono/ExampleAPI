using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Orders.Data;
using ExampleAPI.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ExampleAPI.Orders.Queries;

public class Get {

    public record Query(Guid OrderId) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly IDbConnection _connection;

        public Handler(NpgsqlOrderConnectionFactory factory) {
            _connection = factory.CreateConnection();
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT id, name FROM orders WHERE id = @OrderId;";
            OrderDTO? order = await _connection.QuerySingleOrDefaultAsync<OrderDTO>(query, request);

            if (order is null) {
                return new NotFoundObjectResult($"Order with id '{request.OrderId}' not found.");
            }

            const string itemQuery = "SELECT id, name, qty FROM ordereditems WHERE orderid = @OrderId;";
            order.Items = await _connection.QueryAsync<OrderedItemDTO>(itemQuery, request);

            return new OkObjectResult(order);

        }
        
    }
}
