namespace ExampleAPI.Companies.DTO;

public class CompanyDTO {

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public AddressDTO Address { get; set; } = new();

}