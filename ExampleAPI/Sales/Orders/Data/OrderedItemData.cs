namespace ExampleAPI.Sales.Orders.Data;

public class OrderedItemData {

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Qty { get; set; }

}