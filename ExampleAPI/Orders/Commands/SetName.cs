﻿using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using ExampleAPI.Orders.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Orders.Commands;

public class SetName {

    public record Command(int OrderId, string NewName) : IRequest<IActionResult>;

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly IRepository<Order> _repository;

        public Handler(IRepository<Order> repository) {
            _repository = repository;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {
            var order = await _repository.Get(request.OrderId);

            if (order is null) {
                return new NotFoundResult();
            }

            order.SetName(request.NewName);

            await _repository.Save(order);

            var itemDTOs = new List<OrderedItemDTO>();
            foreach (var item in order.Items.Where(i => i.Id > 0)) {
                itemDTOs.Add(new() {
                    Id = item.Id,
                    Name = item.Name,
                    Qty = item.Qty,
                });
            }

            var orderDto = new OrderDTO() {
                Id = order.Id,
                Name = order.Name,
                Items = itemDTOs
            };

            return new OkObjectResult(orderDto);
        }
    }

}