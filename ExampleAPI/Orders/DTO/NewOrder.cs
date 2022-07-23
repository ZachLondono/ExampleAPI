namespace ExampleAPI.Orders.DTO;

public class NewOrder {

    public string Name { get; set; } = string.Empty;

    public IEnumerable<NewOrderedItem> NewItems { get; set; } = Enumerable.Empty<NewOrderedItem>();

}
