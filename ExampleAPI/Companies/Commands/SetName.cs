using ExampleAPI.Common;
using ExampleAPI.Companies.Domain;
using ExampleAPI.Companies.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Companies.Commands;

public class SetName {

    public record Command(HttpContext Context, Guid CompanyId, NewCompanyName NewName) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {
        
        private readonly IRepository<Company> _repository;

        public Handler(IRepository<Company> repository) {
            _repository = repository;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var company = await _repository.Get(request.CompanyId);

            if (company is null) {
                return new NotFoundObjectResult($"Company with Id {request.CompanyId} not found");
            }

            var etag = request.Context.Request.Headers.ETag;
            if (etag.Count > 0) {

                try {
                    var version = int.Parse(etag.ToString());

                    if (version != company.Version)
                        return new StatusCodeResult(412);

                } catch (FormatException) {
                    // Log invalid etag
                }

            }

            company.SetName(request.NewName.Name);
            await _repository.Save(company);

            var addrDto = new AddressDTO();

            if (company.Address is not null) {

                addrDto.Line1 = company.Address.Line1;
                addrDto.Line2 = company.Address.Line2;
                addrDto.City = company.Address.City;
                addrDto.State = company.Address.State;
                addrDto.Zip = company.Address.Zip;

            }

            request.Context.Response.Headers.ETag = company.Version.ToString();

            return new OkObjectResult(new CompanyDTO() {
                Id = company.Id,
                Version = company.Version,
                Name = company.Name,
                Address = addrDto
            });

        }
    }

}
