using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using ExampleAPI.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Orders.Commands;

public class AdjustItemQty {

    public record Command(HttpContext Context, Guid OrderId, Guid ItemId, OrderedItemQtyAdjustment ItemAdjustment) : EndpointRequest(Context);

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

            try {
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
            } catch {
                // log that header could not be read
            }

            OrderedItem? item = order.Items.SingleOrDefault(i => i.Id == request.ItemId);
            if (item is null) {
                return new NotFoundObjectResult($"Item with id '{request.ItemId}' not found.");
            }

            item.AdjustQty(request.ItemAdjustment.NewQty);
            await _repository.Save(order);

            item = order.Items.SingleOrDefault(i => i.Id == request.ItemId);
            if (item is null) {
                return new NotFoundObjectResult($"Item with id '{request.ItemId}' not found.");
            }

            var itemDto = new OrderedItemDTO() {
                Id = item.Id,
                Name = item.Name,
                Qty = item.Qty
            };

            try { 
                request.Context.Response.Headers.ETag = order.Version.ToString();
            } catch {
                // log that header could not be set
            }

            return new OkObjectResult(itemDto);
        }
    }

}