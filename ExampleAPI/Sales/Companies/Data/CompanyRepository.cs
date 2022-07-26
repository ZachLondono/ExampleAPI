using ExampleAPI.Sales.Companies.Domain;
using MediatR;
using Dapper;
using System.Data;
using ExampleAPI.Common.Data;

namespace ExampleAPI.Sales.Companies.Data;

public class CompanyRepository : ICompanyRepository {

    private readonly IDbConnection _connection;
    private readonly IPublisher _publisher;

    public CompanyRepository(NpgsqlOrderConnectionFactory factory, IPublisher publisher) {
        _connection = factory.CreateConnection();
        _publisher = publisher;
    }

    public async Task AddAsync(Company entity) {

        const string query = "INSERT INTO companies (id, name) VALUES (@Id, @Name);";

        await _connection.ExecuteAsync(query, new { entity.Id, entity.Name });

        await entity.PublishEvents(_publisher);

    }

    public async Task<Company?> GetAsync(Guid id) {

        const string query = "SELECT companies.id, name, line1, line2, city, state, zip, (SELECT version FROM events WHERE companies.id = streamid ORDER BY version DESC LIMIT 1) FROM companies WHERE companies.id = @Id;";

        var companyData = await _connection.QuerySingleOrDefaultAsync<CompanyData>(sql: query, param: new { Id = id });

        if (companyData is null) return null;

        Address address = new();
        if (companyData.Line1 is not null &&
            companyData.City is not null &&
            companyData.State is not null &&
            companyData.Zip is not null) {
            address = new Address(companyData.Line1, companyData.Line2 ?? "", companyData.City, companyData.State, companyData.Zip);
        }

        var company = new Company(companyData.Id, companyData.Version, companyData.Name, address);

        return company;

    }

    public async Task<IEnumerable<Company>> GetAllAsync() {

        const string query = "SELECT companies.id, name, line1, line2, city, state, zip, (SELECT version FROM events WHERE companies.id = streamid ORDER BY version DESC LIMIT 1) FROM companies;";

        var companies = await _connection.QueryAsync<CompanyData, Address, Company>(sql: query, map: (c, a) => new Company(c.Id, c.Version, c.Name, a), splitOn: "line1");

        return companies;

    }

    public async Task RemoveAsync(Company entity) {

        const string command = "DELETE FROM companies WHERE id = @Id;";

        await _connection.ExecuteAsync(command, new { entity.Id });

    }

    public async Task UpdateAsync(Company entity) {

        _connection.Open();
        var trx = _connection.BeginTransaction();

        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            if (domainEvent is Events.NameChangedEvent nameChanged) {

                const string command = "UPDATE companies SET name = @Name WHERE id = @Id;";

                await _connection.ExecuteAsync(command, new {
                    entity.Name,
                    entity.Id
                }, trx);

            } else if (domainEvent is Events.AddressChangedEvent addressChanged) {

                const string command = "UPDATE companies SET line1 = @Line1, line2 = @Line2, city = @City, state = @State, zip = @Zip WHERE id = @Id;";

                await _connection.ExecuteAsync(command, new {
                    entity.Id,
                    addressChanged.NewAddress.Line1,
                    addressChanged.NewAddress.Line2,
                    addressChanged.NewAddress.City,
                    addressChanged.NewAddress.State,
                    addressChanged.NewAddress.Zip,
                }, trx);

            }

        }

        trx.Commit();
        _connection.Close();

        await entity.PublishEvents(_publisher);

    }

    ~CompanyRepository() {
        _connection.Dispose();
    }

}
