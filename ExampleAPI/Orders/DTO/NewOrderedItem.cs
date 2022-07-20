namespace ExampleAPI.Orders.DTO;

/// <summary>
/// Represents a new ordered item to be added to an order
/// </summary>
public class NewOrderedItem {

    public string Name { get; set; } = string.Empty;

    public int Qty { get; set; }

}