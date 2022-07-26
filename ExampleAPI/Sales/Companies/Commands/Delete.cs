using ExampleAPI.Common.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExampleAPI.Sales.Data;

namespace ExampleAPI.Sales.Companies.Commands;

public class Delete {

    public record Command(HttpContext Context, Guid CompanyId) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly SalesUnitOfWork _work;

        public Handler(SalesUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var company = await _work.Companies.Get(request.CompanyId);

            if (company is null) {
                return new NotFoundObjectResult($"Company with Id {request.CompanyId} not found");
            }

            _work.Companies.Remove(company);
            await _work.Complete();

            return new NoContentResult();

        }
    }

}
