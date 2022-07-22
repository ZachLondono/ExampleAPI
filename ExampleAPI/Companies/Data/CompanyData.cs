namespace ExampleAPI.Companies.Data;

public class CompanyData {

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? Line1 { get; set; }
    
    public string? Line2 { get; set; }

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Zip { get; set; }

}
