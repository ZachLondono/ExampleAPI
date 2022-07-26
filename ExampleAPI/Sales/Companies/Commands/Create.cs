using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Companies.Domain;
using ExampleAPI.Sales.Companies.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Sales.Companies.Commands;

public class Create {

    public record Command(HttpContext Context, NewCompany NewCompany) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly IRepository<Company> _repository;

        public Handler(IRepository<Company> repository) {
            _repository = repository;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var name = request.NewCompany.Name;
            var addr = request.NewCompany.Address;
            var company = Company.Create(name, new(addr.Line1, addr.Line2, addr.City, addr.State, addr.Zip));

            await _repository.Add(company);

            var companyDto = new CompanyDTO() {
                Id = company.Id,
                Version = company.Version,
                Name = company.Name,
                Address = new() {
                    Line1 = company.Address.Line1,
                    Line2 = company.Address.Line2,
                    City = company.Address.City,
                    State = company.Address.State,
                    Zip = company.Address.Zip
                }
            };

            try { 
                request.Context.Response.Headers.ETag = company.Version.ToString();
            } catch {
                // log that header could not be set
            }

            return new CreatedResult($"/companies/{companyDto.Id}", companyDto);

        }
    }

}
