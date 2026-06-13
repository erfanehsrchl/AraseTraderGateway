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
    private readonly IOrderGrpcGatewayService _orderGrpcGatewayService;
    private readonly IOrderGatewayService _orderGatewayService;

    public OrdersController(
        IOrderGatewayService orderGatewayService,
        IOrderGrpcGatewayService orderGrpcGatewayService)
    {
        _orderGatewayService = orderGatewayService;
        _orderGrpcGatewayService = orderGrpcGatewayService;
    }

    /// <summary>
    /// Retrieves an order through the Gateway by delegating the query to the OrderService order gRPC API.
    /// </summary>
    [HttpGet(OrdersUriConstants.GetByTrackingId)]
    public async Task<ActionResult<GetOrderByTrackingIdOutVm>> GetOrderByTrackingId(
        Guid trackingId,
        CancellationToken cancellationToken)
    {
        if (trackingId == Guid.Empty)
        {
            return BadRequest("trackingId must not be empty.");
        }

        var result = await _orderGrpcGatewayService.GetOrderByTrackingIdAsync(trackingId, cancellationToken);

        return Ok(result.Adapt<GetOrderByTrackingIdOutVm>());
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
