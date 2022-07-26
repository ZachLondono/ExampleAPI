namespace ExampleAPI.Sales.Companies.Data;

public class CompanyData {

    public Guid Id { get; set; }

    public int Version { get; init; }

    public string Name { get; set; } = string.Empty;

    public string Line1 { get; set; } = string.Empty;

    public string Line2 { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Zip { get; set; } = string.Empty;

}
