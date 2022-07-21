namespace ExampleAPI.Orders.DTO;

public class OrderDTO {

    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public IEnumerable<OrderedItemDTO> Items { get; set; } = Enumerable.Empty<OrderedItemDTO>();

}