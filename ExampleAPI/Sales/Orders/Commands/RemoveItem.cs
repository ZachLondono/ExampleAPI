using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Orders.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Sales.Orders.Commands;

public class RemoveItem {

    public record Command(HttpContext Context, Guid OrderId, Guid ItemId) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly SalesUnitOfWork _work;

        public Handler(SalesUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            //TODO: return a representation of the current order resource, rather than no content

            var order = await _work.Orders.GetAsync(request.OrderId);

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

            var item = order.Items.FirstOrDefault(i => i.Id == request.ItemId);

            if (item is null) {
                return new NotFoundObjectResult($"OrderedItem with id '{request.ItemId}' not found.");
            }

            order.RemoveItem(item);

            await _work.Orders.UpdateAsync(order);
            await _work.CommitAsync();

            try { 
                request.Context.Response.Headers.ETag = order.Version.ToString();
            } catch {
                // log that header could not be set
            }

            return new NoContentResult();

        }

    }
}