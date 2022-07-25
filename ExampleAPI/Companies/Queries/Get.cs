using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Companies.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Companies.Queries;

public class Get {

    public record Query(HttpContext Context, Guid CompanyId) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly NpgsqlOrderConnectionFactory _factory;

        public Handler(NpgsqlOrderConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            using var connection = _factory.CreateConnection();

            const string query = "SELECT companies.id, name, (SELECT version FROM events WHERE companies.id = streamid ORDER BY version DESC LIMIT 1), line1, line2, city, state, zip FROM companies WHERE companies.id = @CompanyId;";

            var companies = await connection.QueryAsync<CompanyDTO, AddressDTO, CompanyDTO>(query,
                param: new { request.CompanyId },
                map: (c, a) => {
                    c.Address = a;
                    return c;
                },
                splitOn: "line1"
            );

            var company = companies.FirstOrDefault();

            if (company is null) {
                return new NotFoundObjectResult($"Company with id {request.CompanyId} not found");
            }

            try { 
                request.Context.Response.Headers.ETag = company.Version.ToString();
            } catch {
                // log that header could not be set
            }

            return new OkObjectResult(company);

        }
    }

}