using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Companies.Domain;
using ExampleAPI.Sales.Companies.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ExampleAPI.Sales.Data;

namespace ExampleAPI.Sales.Companies.Commands;

public class Create {

    public record Command(HttpContext Context, NewCompany NewCompany) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly SalesUnitOfWork _work;

        public Handler(SalesUnitOfWork work) {
            _work = work;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            var name = request.NewCompany.Name;
            var addr = request.NewCompany.Address;
            var company = Company.Create(name, new(addr.Line1, addr.Line2, addr.City, addr.State, addr.Zip));

            _work.Companies.Add(company);
            await _work.Complete();

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
