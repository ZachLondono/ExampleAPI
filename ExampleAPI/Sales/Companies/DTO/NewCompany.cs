namespace ExampleAPI.Sales.Companies.DTO;

public class NewCompany {

    public string Name { get; set; } = string.Empty;

    public AddressDTO Address { get; set; } = new();

}
