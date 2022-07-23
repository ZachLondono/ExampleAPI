using Dapper;
using ExampleAPI.Common;
using ExampleAPI.Companies.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Companies.Queries;

public class Get {

    public record Query(Guid CompanyId) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Query, IActionResult> {

        private readonly NpgsqlOrderConnectionFactory _factory;

        public Handler(NpgsqlOrderConnectionFactory factory) {
            _factory = factory;
        }

        public async Task<IActionResult> Handle(Query request, CancellationToken cancellationToken) {

            var connection = _factory.CreateConnection();

            const string query = "SELECT id, name, line1, line2, city, state, zip FROM companies WHERE id = @CompanyId;";

            var companies = await connection.QueryAsync<CompanyDTO, AddressDTO?, CompanyDTO>(query,
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

            return new OkObjectResult(company);

        }
    }

}