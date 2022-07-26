using ExampleAPI.Common.Domain;
using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Orders.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExampleAPI.Sales.Data;

namespace ExampleAPI.Sales.Orders.Commands;

public class Delete {

    public record Command(HttpContext Context, Guid OrderId) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly SalesUnitOfWork _work;

        public Handler(SalesUnitOfWork work) {
            _work = work;
        }

        ~Handler() {
            _work.Dispose();
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {
            var order = await _work.Orders.Get(request.OrderId);

            if (order is null) {
                return new NotFoundObjectResult($"Order with id '{request.OrderId}' not found.");
            }

            _work.Orders.Remove(order);
            await _work.Complete();

            return new NoContentResult();
        }
    }

}