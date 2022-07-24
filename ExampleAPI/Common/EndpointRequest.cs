using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Common;

public record EndpointRequest(HttpContext Context) : IRequest<IActionResult>;
