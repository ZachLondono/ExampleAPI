using Dapper;
using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace ExampleAPI.Sales.Orders.Queries;

public class GetAll {

    public record Query(HttpContext Context) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly NpgsqlSalesConnectionFactory _factory;

        public Handler(NpgsqlSalesConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            using var connection = _factory.CreateConnection();

            const string query = "SELECT orders.id, name, (SELECT version FROM events WHERE orders.id = streamid ORDER BY version DESC LIMIT 1) FROM orders;";
            var orders = await connection.QueryAsync<OrderDTO>(query);

            foreach (var order in orders) {
                var items = await GetItemsFromOrderId(connection, order.Id);
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