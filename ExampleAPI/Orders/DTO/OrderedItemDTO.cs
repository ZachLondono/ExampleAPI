namespace ExampleAPI.Orders.DTO;

/// <summary>
/// Represents an ordered item which is part of an order
/// </summary>
public class OrderedItemDTO {

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int Qty { get; set; }

}
