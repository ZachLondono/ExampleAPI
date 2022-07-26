namespace ExampleAPI.Sales.Companies.Domain;

public record Address {

    public string Line1 { get; init; }

    public string Line2 { get; init; }

    public string City { get; init; }

    public string State { get; init; }

    public string Zip { get; init; }

    public Address(string line1, string line2, string city, string state, string zip) {
        Line1 = line1;
        Line2 = line2;
        City = city;
        State = state;
        Zip = zip;
    }

    public Address() {
        Line1 = string.Empty;
        Line2 = string.Empty;
        City = string.Empty;
        State = string.Empty;
        Zip = string.Empty;
    }

}
