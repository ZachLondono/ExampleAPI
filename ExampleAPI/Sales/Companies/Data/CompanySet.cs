using ExampleAPI.Common.Data;
using ExampleAPI.Sales.Companies.Domain;
using Dapper;
using System.Data;

namespace ExampleAPI.Sales.Companies.Data;

public class CompanySet : PersistanceSet<Company> {

    private readonly IDbConnection _connection;

    public CompanySet(NpgsqlOrderConnectionFactory factory) {
        _connection = factory.CreateConnection();
    }

    public override async Task<Company?> Get(Guid id) {

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

    public override async Task SaveChanges() {

        _connection.Open();
        var trx = _connection.BeginTransaction();

        foreach(var entity in Entities) {

            await SaveEntity(entity, _connection, trx);

        }

        trx.Commit();
        _connection.Close();

        //foreach (var entity in Entities) {
        //    await entity.PublishEvents(_publisher);
        //}

    }

    private static async Task SaveEntity(Company entity, IDbConnection connection, IDbTransaction trx) {

        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            if (domainEvent is Events.CompanyCreatedEvent created) {

                const string query = "INSERT INTO companies (id, name) VALUES (@Id, @Name);";

                await connection.ExecuteAsync(query, new { entity.Id, created.Name }, trx);

            } if (domainEvent is Events.NameChangedEvent nameChanged) {

                const string command = "UPDATE companies SET name = @Name WHERE id = @Id;";

                await connection.ExecuteAsync(command, new {
                    nameChanged.Name,
                    entity.Id
                }, trx);

            } else if (domainEvent is Events.AddressChangedEvent addressChanged) {

                const string command = "UPDATE companies SET line1 = @Line1, line2 = @Line2, city = @City, state = @State, zip = @Zip WHERE id = @Id;";

                await connection.ExecuteAsync(command, new {
                    entity.Id,
                    addressChanged.NewAddress.Line1,
                    addressChanged.NewAddress.Line2,
                    addressChanged.NewAddress.City,
                    addressChanged.NewAddress.State,
                    addressChanged.NewAddress.Zip,
                }, trx);

            }

        }

    }

    ~CompanySet() {
        _connection.Dispose();
    }

}
