using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Companies.Domain;
using ExampleAPI.Sales.Companies.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Sales.Companies.Commands;

public class SetAddress {

    public record Command(HttpContext Context, Guid CompanyId, AddressDTO NewAddress) : EndpointRequest(Context);

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

            try {
                var ifMatch = request.Context.Request.Headers.IfMatch;
                if (ifMatch.Count > 0) {

                    try {

                        if (!ifMatch.Contains(company.Version.ToString()))
                            return new StatusCodeResult(412);

                    } catch (FormatException) {
                        // Log invalid etag
                    }

                }
            } catch {
                // log that header could not be read
            }

            company.SetAddress(new Address(request.NewAddress.Line1,
                                            request.NewAddress.Line2,
                                            request.NewAddress.City,
                                            request.NewAddress.State,
                                            request.NewAddress.Zip));

            await _work.Companies.UpdateAsync(company);
            await _work.CommitAsync();

            var addrDto = new AddressDTO();

            if (company.Address is not null) {

                addrDto.Line1 = company.Address.Line1;
                addrDto.Line2 = company.Address.Line2;
                addrDto.City = company.Address.City;
                addrDto.State = company.Address.State;
                addrDto.Zip = company.Address.Zip;

            }

            try { 
                request.Context.Response.Headers.ETag = company.Version.ToString();
            } catch {
                // log that header could not be set
            }

            return new OkObjectResult(new CompanyDTO() {
                Id = company.Id,
                Version = company.Version,
                Name = company.Name,
                Address = addrDto
            });

        }
    }

}
