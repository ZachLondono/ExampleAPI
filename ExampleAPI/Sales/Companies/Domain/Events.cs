using ExampleAPI.Common.Domain;
using ExampleAPI.Common.Data;
using System.Text.Json.Serialization;

namespace ExampleAPI.Sales.Companies.Domain;

public static class Events {

    public record CompanyEvent([property: JsonIgnore] Guid CompanyId) : DomainEvent(CompanyId);

    public record CompanyCreatedEvent(Guid CompanyId, string Name) : CompanyEvent(CompanyId);

    public record AddressChangedEvent(Guid CompanyId, Address NewAddress) : CompanyEvent(CompanyId);

    public record NameChangedEvent(Guid CompanyId, string Name) : CompanyEvent(CompanyId);

}