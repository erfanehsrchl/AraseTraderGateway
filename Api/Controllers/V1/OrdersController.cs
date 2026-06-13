using Api.UriConstants.V1;
using Api.ViewModels.V1;
using Asp.Versioning;
using Application.DTOs;
using Application.Interfaces.V1;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.V1;

/// <summary>
/// Exposes version 1 Gateway order endpoints and delegates order acceptance to the application layer.
/// The controller keeps HTTP concerns separate from Outbox-based message creation.
/// </summary>
[ApiController]
[ApiVersion(1.0)]
[Route(OrdersUriConstants.Route)]
public class OrdersController : ControllerBase
{
    private readonly IOrderGatewayService _orderGatewayService;

    public OrdersController(IOrderGatewayService orderGatewayService)
    {
        _orderGatewayService = orderGatewayService;
    }

    /// <summary>
    /// Accepts an order request through the Gateway and stages a CreateOrder integration event in the outbox
    /// for reliable RabbitMQ delivery.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<AddOrderOutVm>> AddOrder(
        AddOrderInVm request,
        [FromHeader(Name = "Idempotency-Key")] string? idempotencyKey,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            return BadRequest("Idempotency-Key header is required.");
        }

        var input = request.Adapt<AddOrderInDto>();
        input.IdempotencyKey = idempotencyKey;
        var result = await _orderGatewayService.AddOrderAsync(input, cancellationToken);

        return Ok(result.Adapt<AddOrderOutVm>());
    }
}
