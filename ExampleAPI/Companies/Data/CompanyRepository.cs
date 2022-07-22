﻿using ExampleAPI.Common;
using ExampleAPI.Companies.Domain;
using MediatR;
using Dapper;
using System.Data;

namespace ExampleAPI.Companies.Data;

public class CompanyRepository : IRepository<Company> {
    
    private readonly IDbConnection _connection;
    private readonly IPublisher _publisher;

    public CompanyRepository(NpgsqlOrderConnectionFactory factory, IPublisher publisher) {
        _connection = factory.CreateConnection();
        _publisher = publisher;
    }

    public async Task<Company> Create() {

        const string query = "INSERT INTO companies (name) VALUES (@Name) RETURNING id;";

        const string defaultName = "New Company";

        int newId = await _connection.QuerySingleAsync<int>(query, new { Name = defaultName });

        _ = _publisher.Publish(new Events.CompanyCreatedEvent(newId));

        return new(newId, defaultName, null);

    }

    public async Task<Company?> Get(int id) {

        const string query = "SELECT id, name, line1, line2, city, state, zip FROM companies WHERE id = @Id;";

        var companyData = await _connection.QuerySingleOrDefaultAsync<CompanyData>(sql: query, param: new { Id = id });

        if (companyData is null) return null;

        Address? address = null;
        if (companyData.Line1 is not null &&
            companyData.City is not null &&
            companyData.State is not null &&
            companyData.Zip is not null) {
            address = new Address(companyData.Line1, companyData.Line2 ?? "", companyData.City, companyData.State, companyData.Zip);
        }

        var company = new Company(companyData.Id, companyData.Name, address);

        return company;

    }

    public async Task<IEnumerable<Company>> GetAll() {

        const string query = "SELECT id, name, line1, line2, city, state, zip FROM companies;";

        var companies = await _connection.QueryAsync<CompanyData, Address, Company>(sql: query, map: (c, a) => new Company(c.Id, c.Name, a), splitOn: "line1");

        return companies;

    }

    public async Task Remove(Company entity) {

        const string command = "DELETE FROM companies WHERE id = @Id;";

        await _connection.ExecuteAsync(command, new { entity.Id });

    }

    public async Task Save(Company entity) {

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

        entity.PublishEvents(_publisher);

    }

}
