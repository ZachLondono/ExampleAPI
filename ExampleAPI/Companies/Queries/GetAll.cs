﻿using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Companies.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Companies.Queries;

public class GetAll {

    public record Query() : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly NpgsqlOrderConnectionFactory _factory;

        public Handler(NpgsqlOrderConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            var connection = _factory.CreateConnection();

            const string query = "SELECT id, name, line1, line2, city, state, zip FROM companies;";

            var companies = await connection.QueryAsync<CompanyDTO, AddressDTO, CompanyDTO>(query,
                map: (c, a) => {
                    c.Address = a;
                    return c;
                },
                splitOn: "line1"
            );

            return new OkObjectResult(companies);

        }
    }

}