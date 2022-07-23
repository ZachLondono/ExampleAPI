namespace ExampleAPI.Orders.DTO;

public class OrderDTO {

    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public IEnumerable<OrderedItemDTO> Items { get; set; } = Enumerable.Empty<OrderedItemDTO>();

}