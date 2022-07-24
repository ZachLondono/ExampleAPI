using ExampleAPI.Common;

namespace ExampleAPI.Companies.Domain;

public class Company : Entity {

    public string Name { get; private set; }

    public Address Address { get; private set; }

    public Company(Guid id, string name, Address address) : base(id) {
        Name = name;
        Address = address;
    }

    private Company(string name, Address address) : this(Guid.NewGuid(), name, address) {
        AddEvent(new Events.CompanyCreatedEvent(Id, name));
    }

    public static Company Create(string name, Address address) => new(name, address);

    public void SetAddress(Address address) {
        AddEvent(new Events.AddressChangedEvent(Id, address));
        Address = address;
    }

    public void SetName(string name) {
        AddEvent(new Events.NameChangedEvent(Id, name));
        Name = name;
    }

}