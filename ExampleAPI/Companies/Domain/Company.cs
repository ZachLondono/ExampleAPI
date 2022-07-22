using ExampleAPI.Common;

namespace ExampleAPI.Companies.Domain;

public class Company : Entity {

    public string Name { get; private set; }

    public Address? Address { get; private set; }

    public Company(int id, string name, Address? address) : base(id) {
        Name = name;
        Address = address;
    }

    public void SetAddress(Address address) {
        AddEvent(new Events.AddressChangedEvent(Id, address));
        Address = address;
    }

    public void SetName(string name) {
        AddEvent(new Events.NameChangedEvent(Id, name));
        Name = name;
    }

}