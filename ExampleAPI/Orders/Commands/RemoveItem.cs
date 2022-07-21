﻿using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Orders.Commands;

public class RemoveItem {

    public record Command(int OrderId, int ItemId) : IRequest<IActionResult>;

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

            var item = order.Items.FirstOrDefault(i => i.Id == request.ItemId);

            if (item is null) {
                return new NotFoundObjectResult($"OrderedItem with id '{request.ItemId}' not found.");
            }

            order.RemoveItem(item);

            await _repository.Save(order);

            return new NoContentResult();

        }

    }
}