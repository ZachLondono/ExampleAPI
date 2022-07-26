namespace ExampleAPI.Sales.Companies.DTO;

public class AddressDTO {

    public string Line1 { get; set; } = string.Empty;

    public string Line2 { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string Zip { get; set; } = string.Empty;

    public override bool Equals(object? address) {
        
        if(address == null) return false;
        if (address is not AddressDTO) return false;

        AddressDTO dto = (AddressDTO)address;

        return dto.Line1.Equals(Line1)
            && dto.Line2.Equals(Line2)
            && dto.City.Equals(City)
            && dto.State.Equals(State)
            && dto.Zip.Equals(Zip);

    }

    public override int GetHashCode() {
        return HashCode.Combine(Line1, Line2, City, State, Zip);
    }
}