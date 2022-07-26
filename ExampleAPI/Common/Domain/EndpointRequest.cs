using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Common.Domain;

public record EndpointRequest(HttpContext Context) : IRequest<IActionResult>;
