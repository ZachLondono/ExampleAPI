using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using ExampleAPI.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Orders.Commands;

public class Create {

    public record Command(HttpContext Context, NewOrder NewOrder) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly IRepository<Order> _repository;

        public Handler(IRepository<Order> repository) {
            _repository = repository;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var order = Order.Create(request.NewOrder.Name);

            if (request.NewOrder.NewItems.Any()) {
                foreach (var item in request.NewOrder.NewItems) {
                    order.AddItem(item.Name, item.Qty);
                }
            }

            await _repository.Add(order);

            var itemDTOs = new List<OrderedItemDTO>();

            foreach (var item in order.Items) {
                itemDTOs.Add(new() {
                    Id = item.Id,
                    Name = item.Name,
                    Qty = item.Qty
                });
            }

            var newOrder = new OrderDTO() {
                Id = order.Id,
                Version = order.Version,
                Name = order.Name,
                Items = itemDTOs
            };

            request.Context.Response.Headers.ETag = order.Version.ToString();

            return new CreatedResult($"/orders/{newOrder.Id}", newOrder);

        }

    }


}
