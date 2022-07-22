using ExampleAPI.Common;

namespace ExampleAPI.Companies.Domain;

public static class Events {

    public record CompanyEvent() : DomainEvent;

    public record CompanyCreatedEvent(int CompanyId) : CompanyEvent;

    public record AddressChangedEvent(int CompanyId, Address NewAddress) : CompanyEvent;

    public record NameChangedEvent(int CompanyId, string Name) : CompanyEvent;

}