namespace ExampleAPI.Orders.DTO;

/// <summary>
/// Represents an ordered item which quantity is to be adjusted
/// </summary>
public class OrderedItemQtyAdjustment {

    public int Id { get; set; }

    public int NewQty { get; set; }

}