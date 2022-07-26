using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Orders.Domain;
using ExampleAPI.Sales.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Sales.Orders.Commands;

public class AddItem {

    public record Command(HttpContext Context, Guid OrderId, NewOrderedItem NewItem) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ISalesUnitOfWork _work;

        public Handler(ISalesUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

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

            var newItem = order.AddItem(request.NewItem.Name, request.NewItem.Qty);

            await _work.Orders.UpdateAsync(order);
            await _work.CommitAsync();

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

            try { 
                request.Context.Response.Headers.ETag = order.Version.ToString();
            } catch {
                // log that header could not be set
            }

            return new CreatedResult($"/orders/{orderDto.Id}/items/{newItem.Id}", orderDto);

        }
    }

}
