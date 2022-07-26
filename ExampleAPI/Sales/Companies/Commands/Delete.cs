using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Companies.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Sales.Companies.Commands;

public class Delete {

    public record Command(HttpContext Context, Guid CompanyId) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly ISalesUnitOfWork _work;

        public Handler(ISalesUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var company = await _work.Companies.GetAsync(request.CompanyId);

            if (company is null) {
                return new NotFoundObjectResult($"Company with Id {request.CompanyId} not found");
            }

            await _work.Companies.RemoveAsync(company);
            await _work.CommitAsync();

            return new NoContentResult();

        }
    }

}
