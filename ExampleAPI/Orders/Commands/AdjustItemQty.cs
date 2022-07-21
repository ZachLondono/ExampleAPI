using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using ExampleAPI.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Orders.Commands;

public class AdjustItemQty {

    public record Command(int OrderId, OrderedItemQtyAdjustment ItemAdjustment) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly IRepository<Order> _repository;

        public Handler(IRepository<Order> repository) {
            _repository = repository;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {
            var order = await _repository.Get(request.OrderId);

            if (order is null) {
                return new NotFoundObjectResult($"Order with id '{request.OrderId}' not found.");
            }

            OrderedItem? item = order.Items.SingleOrDefault(i => i.Id == request.ItemAdjustment.Id);
            if (item is null) {
                return new NotFoundObjectResult($"Item with id '{request.ItemAdjustment.Id}' not found.");
            }

            item.AdjustQty(request.ItemAdjustment.NewQty);
            await _repository.Save(order);

            item = order.Items.SingleOrDefault(i => i.Id == request.ItemAdjustment.Id);
            if (item is null) {
                return new NotFoundObjectResult($"Item with id '{request.ItemAdjustment.Id}' not found.");
            }

            var itemDto = new OrderedItemDTO() {
                Id = item.Id,
                Name = item.Name,
                Qty = item.Qty
            };

            return new OkObjectResult(itemDto);
        }
    }

}