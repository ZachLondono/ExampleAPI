using ExampleAPI.Common;

namespace ExampleAPI.Companies.Domain;

public static class Events {

    public record CompanyEvent() : DomainEvent;

    public record CompanyCreatedEvent(Guid CompanyId) : CompanyEvent;

    public record AddressChangedEvent(Guid CompanyId, Address NewAddress) : CompanyEvent;

    public record NameChangedEvent(Guid CompanyId, string Name) : CompanyEvent;

}