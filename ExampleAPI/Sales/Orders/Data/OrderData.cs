namespace ExampleAPI.Sales.Orders.Data;

public class OrderData {

    public Guid Id { get; set; }

    public int Version { get; set; }

    public string Name { get; set; } = string.Empty;

}
