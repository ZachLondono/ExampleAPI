using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Orders.Domain;
using ExampleAPI.Sales.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExampleAPI.Sales.Data;

namespace ExampleAPI.Sales.Orders.Commands;

public class Create {

    public record Command(HttpContext Context, NewOrder NewOrder) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly SalesUnitOfWork _work;

        public Handler(SalesUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var order = Order.Create(request.NewOrder.Name);

            if (request.NewOrder.NewItems.Any()) {
                foreach (var item in request.NewOrder.NewItems) {
                    order.AddItem(item.Name, item.Qty);
                }
            }

            _work.Orders.Add(order);
            await _work.Complete();

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

            try { 
                request.Context.Response.Headers.ETag = order.Version.ToString();
            } catch {
                // log that header could not be set
            }

            return new CreatedResult($"/orders/{newOrder.Id}", newOrder);

        }

    }


}
