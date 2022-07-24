﻿using ExampleAPI.Common;
using ExampleAPI.Orders.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Orders.Commands;

public class RemoveItem {

    public record Command(HttpContext Context, Guid OrderId, Guid ItemId) : EndpointRequest(Context);

    public class Handler : IRequestHandler<Command, IActionResult> {

        private readonly IRepository<Order> _repository;

        public Handler(IRepository<Order> repository) {
            _repository = repository;
        }

        public async Task<IActionResult> Handle(Command request, CancellationToken cancellationToken) {

            //TODO: return a representation of the current order resource, rather than no content

            var order = await _repository.Get(request.OrderId);

            if (order is null) {
                return new NotFoundObjectResult($"Order with id '{request.OrderId}' not found.");
            }

            var etag = request.Context.Request.Headers.ETag;
            if (etag.Count > 0) {

                try {
                    var version = int.Parse(etag.ToString());

                    if (version != order.Version)
                        return new StatusCodeResult(412);

                } catch (FormatException) {
                    // Log invalid etag
                }

            }

            var item = order.Items.FirstOrDefault(i => i.Id == request.ItemId);

            if (item is null) {
                return new NotFoundObjectResult($"OrderedItem with id '{request.ItemId}' not found.");
            }

            order.RemoveItem(item);

            await _repository.Save(order);

            request.Context.Response.Headers.ETag = order.Version.ToString();

            return new NoContentResult();

        }

    }
}