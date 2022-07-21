using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using ExampleAPI.Orders.DTO;
using MediatR;

namespace ExampleAPI.Orders.Commands;

public class Create {

    public record Command(NewOrder NewOrder) : IRequest<OrderDTO>;

    public class Handler : IRequestHandler<Command, OrderDTO> {

        private readonly IRepository<Order> _repository;

        public Handler(IRepository<Order> repository) {
            _repository = repository;
        }

        public async Task<OrderDTO> Handle(Command request, CancellationToken cancellationToken) {
            
            var order = await _repository.Create();
            order.SetName(request.NewOrder.Name);
            foreach (var item in request.NewOrder.NewItems) {
                order.AddItem(item.Name, item.Qty);
            }

            order = await _repository.Save(order);

            var itemDTOs = new List<OrderedItemDTO>();

            foreach (var item in order.Items) {
                itemDTOs.Add(new() {
                    Id = item.Id,
                    Name = item.Name,
                    Qty = item.Qty
                });
            }

            return new OrderDTO() {
                Id = order.Id,
                Name = order.Name,
                Items = itemDTOs
            };

        }

    }


}
