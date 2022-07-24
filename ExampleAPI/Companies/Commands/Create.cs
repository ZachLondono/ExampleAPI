﻿using ExampleAPI.Common;
using ExampleAPI.Companies.Domain;
using ExampleAPI.Companies.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Companies.Commands;

public class Create {

    public record Command(NewCompany NewCompany) : IRequest<IActionResult>;

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
                Name = company.Name,
                Address = new() {
                    Line1 = company.Address.Line1,
                    Line2 = company.Address.Line2,
                    City = company.Address.City,
                    State = company.Address.State,
                    Zip = company.Address.Zip
                }
            };

            return new CreatedResult($"/companies/{companyDto.Id}", companyDto);

        }
    }

}
