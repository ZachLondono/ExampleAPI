using Dapper;
using ExampleAPI.Orders.Data;
using ExampleAPI.Orders.DTO;
using MediatR;
using System.Data;

namespace ExampleAPI.Orders.Queries;

public class GetAll {

    public record Query() : IRequest<IEnumerable<OrderDTO>>;

    public class Handler : IRequestHandler<Query, IEnumerable<OrderDTO>> {

        private readonly IDbConnection _connection;

        public Handler(NpgsqlOrderConnectionFactory factory) {
            _connection = factory.CreateConnection();
        }

        public async Task<IEnumerable<OrderDTO>> Handle(Query request, CancellationToken cancellationToken) {

            const string query = "SELECT id, name FROM orders;";
            var orders = await _connection.QueryAsync<OrderDTO>(query);

            foreach (var order in orders) {
                var items = await GetItemsFromOrderId(_connection, order.Id);
                order.Items = items;
            }

            return orders;

        }
        
        private static async Task<IEnumerable<OrderedItemDTO>> GetItemsFromOrderId(IDbConnection connection, int orderId, IDbTransaction? transaction = null) {
            const string itemQuery = "SELECT id, name, qty FROM ordereditems WHERE orderid = @OrderId;";
            return await connection.QueryAsync<OrderedItemDTO>(itemQuery, new { OrderId = orderId }, transaction);
        }

    }


}