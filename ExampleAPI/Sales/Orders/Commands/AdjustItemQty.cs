using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Orders.Domain;
using ExampleAPI.Sales.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExampleAPI.Sales.Data;

namespace ExampleAPI.Sales.Orders.Commands;

public class AdjustItemQty {

    public record Command(HttpContext Context, Guid OrderId, Guid ItemId, OrderedItemQtyAdjustment ItemAdjustment) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly SalesUnitOfWork _work;

        public Handler(SalesUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {
            var order = await _work.Orders.Get(request.OrderId);

            if (order is null) {
                return new NotFoundObjectResult($"Order with id '{request.OrderId}' not found.");
            }

            try {
                var ifMatch = request.Context.Request.Headers.IfMatch;
                if (ifMatch.Count > 0) {

                    try {

                        if (!ifMatch.Contains(order.Version.ToString()))
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
            await _work.Complete();

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