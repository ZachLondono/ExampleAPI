using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Orders.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Sales.Orders.Commands;

public class Delete {

    public record Command(HttpContext Context, Guid OrderId) : EndpointRequest(Context);

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

            await _work.Orders.RemoveAsync(order);
            await _work.CommitAsync();

            return new NoContentResult();
        }
    }

}