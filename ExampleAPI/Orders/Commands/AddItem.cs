using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using ExampleAPI.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Orders.Commands;

public class AddItem {

    public record Command(HttpContext Context, Guid OrderId, NewOrderedItem NewItem) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly IRepository<Order> _repository;

        public Handler(IRepository<Order> repository) {
            _repository = repository;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var order = await _repository.Get(request.OrderId);

            if (order is null) {
                return new NotFoundObjectResult($"Order with id '{request.OrderId}' not found.");
            }

            var etag = request.Context.Request.Headers.ETag;
            if (etag.Count > 0) {

                try { 
                    var version = int.Parse(etag.ToString());

                    if (version != order.Version)
                        return new StatusCodeResult(412);

                } catch (FormatException) {
                    // Log invalid etag
                }

            }

            var newItem = order.AddItem(request.NewItem.Name, request.NewItem.Qty);

            await _repository.Save(order);

            var itemDTOs = new List<OrderedItemDTO>();
            foreach (var item in order.Items) {
                itemDTOs.Add(new() {
                    Id = item.Id,
                    Name = item.Name,
                    Qty = item.Qty,
                });
            }

            var orderDto = new OrderDTO() {
                Id = order.Id,
                Version = order.Version,
                Name = order.Name,
                Items = itemDTOs
            };


            request.Context.Response.Headers.ETag = order.Version.ToString();

            return new CreatedResult($"/orders/{orderDto.Id}/items/{newItem.Id}", orderDto);

        }
    }

}
