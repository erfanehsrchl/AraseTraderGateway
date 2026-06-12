using Api.UriConstants.V1;
using Api.ViewModels.V1;
using Asp.Versioning;
using Application.DTOs;
using Application.Interfaces.V1;
using Mapster;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.V1;

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

    [HttpPost]
    public async Task<ActionResult<AddOrderOutVm>> AddOrder(
        AddOrderInVm request,
        CancellationToken cancellationToken)
    {
        var input = request.Adapt<AddOrderInDto>();
        var result = await _orderGatewayService.AddOrderAsync(input, cancellationToken);

        return Ok(result.Adapt<AddOrderOutVm>());
    }
}
