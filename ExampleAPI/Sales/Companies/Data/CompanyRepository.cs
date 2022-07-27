using ExampleAPI.Sales.Companies.Domain;
using MediatR;
using System.Data;
using ExampleAPI.Common.Data;

namespace ExampleAPI.Sales.Companies.Data;

public class CompanyRepository : ICompanyRepository {

    private readonly IDapperConnection _connection;
    private readonly IDbTransaction _transaction;

    private readonly List<Company> _activeEntities = new();
    public IReadOnlyCollection<Company> ActiveEntities => _activeEntities.AsReadOnly();

    public CompanyRepository(IDapperConnection connection, IDbTransaction transaction) {
        _connection = connection;
        _transaction = transaction;
    }

    public async Task AddAsync(Company entity) {

        const string query = "INSERT INTO companies (id, name) VALUES (@Id, @Name);";

        await _connection.ExecuteAsync(query, new { entity.Id, entity.Name }, _transaction);

        _activeEntities.Add(entity);

    }

    public async Task<Company?> GetAsync(Guid id) {

        const string query = "SELECT companies.id, name, line1, line2, city, state, zip, (SELECT version FROM events WHERE companies.id = streamid ORDER BY version DESC LIMIT 1) FROM companies WHERE companies.id = @Id;";

        var companyData = await _connection.QuerySingleOrDefaultAsync<CompanyData>(sql: query, param: new { Id = id }, _transaction);

        if (companyData is null) return null;

        Address address = new();
        if (companyData.Line1 is not null &&
            companyData.City is not null &&
            companyData.State is not null &&
            companyData.Zip is not null) {
            address = new Address(companyData.Line1, companyData.Line2 ?? "", companyData.City, companyData.State, companyData.Zip);
        }

        var company = new Company(companyData.Id, companyData.Version, companyData.Name, address);

        var existing = _activeEntities.FirstOrDefault(o => o.Id == company.Id);
        if (existing is not null) _activeEntities.Remove(existing);
        _activeEntities.Add(company);

        return company;

    }

    public async Task<IEnumerable<Company>> GetAllAsync() {

        const string query = "SELECT companies.id, name, line1, line2, city, state, zip, (SELECT version FROM events WHERE companies.id = streamid ORDER BY version DESC LIMIT 1) FROM companies;";

        var companies = await _connection.QueryAsync<CompanyData, Address, Company>(sql: query, map: (c, a) => new Company(c.Id, c.Version, c.Name, a), splitOn: "line1", transaction: _transaction);

        foreach (var company in companies) {
            var existing = _activeEntities.FirstOrDefault(o => o.Id == company.Id);
            if (existing is not null) _activeEntities.Remove(existing);
            _activeEntities.Add(company);
        }

        return companies;

    }

    public async Task RemoveAsync(Company entity) {

        const string command = "DELETE FROM companies WHERE id = @Id;";

        await _connection.ExecuteAsync(command, new { entity.Id }, _transaction);

        _activeEntities.Remove(entity);

    }

    public async Task UpdateAsync(Company entity) {

        foreach (var domainEvent in entity.Events.Where(e => !e.IsPublished)) {

            if (domainEvent is Events.NameChangedEvent nameChanged) {

                const string command = "UPDATE companies SET name = @Name WHERE id = @Id;";

                await _connection.ExecuteAsync(command, new {
                    entity.Name,
                    entity.Id
                }, _transaction);

            } else if (domainEvent is Events.AddressChangedEvent addressChanged) {

                const string command = "UPDATE companies SET line1 = @Line1, line2 = @Line2, city = @City, state = @State, zip = @Zip WHERE id = @Id;";

                await _connection.ExecuteAsync(command, new {
                    entity.Id,
                    addressChanged.NewAddress.Line1,
                    addressChanged.NewAddress.Line2,
                    addressChanged.NewAddress.City,
                    addressChanged.NewAddress.State,
                    addressChanged.NewAddress.Zip,
                }, _transaction);

            }

        }

    }

}
