﻿using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Companies.Domain;
using ExampleAPI.Sales.Companies.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Sales.Companies.Commands;

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