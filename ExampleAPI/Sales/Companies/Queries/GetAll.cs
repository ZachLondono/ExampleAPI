﻿using Dapper;
using ExampleAPI.Common.Domain;
using ExampleAPI.Sales.Companies.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Sales.Companies.Queries;

public class GetAll {

    public record Query(HttpContext Context) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly NpgsqlSalesConnectionFactory _factory;

        public Handler(NpgsqlSalesConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            using var connection = _factory.CreateConnection();

            const string query = "SELECT companies.id, name, (SELECT version FROM events WHERE companies.id = streamid ORDER BY version DESC LIMIT 1), line1, line2, city, state, zip FROM companies;";

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