using Api.UriConstants.V1;
using Application.Interfaces.V1;
using Asp.Versioning;
using Contracts.Grpc.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.V1;

[ApiController]
[ApiVersion(1.0)]
[Route(WalletsUriConstants.Route)]
public class WalletsController : ControllerBase
{
    private readonly IWalletGatewayService _walletGatewayService;

    public WalletsController(IWalletGatewayService walletGatewayService)
    {
        _walletGatewayService = walletGatewayService;
    }

    [HttpGet(WalletsUriConstants.GetByCustomerId)]
    public async Task<ActionResult<GetWalletByCustomerIdGrpcResponse>> GetWalletByCustomerId(
        long customerId,
        CancellationToken cancellationToken)
    {
        if (customerId <= 0)
        {
            return BadRequest("customerId must be greater than zero.");
        }

        var result = await _walletGatewayService.GetWalletByCustomerIdAsync(customerId, cancellationToken);

        return Ok(result);
    }

    [HttpGet(WalletsUriConstants.GetTransactionsByWalletId)]
    public async Task<ActionResult<GetWalletTransactionsByWalletIdGrpcResponse>> GetWalletTransactionsByWalletId(
        long walletId,
        CancellationToken cancellationToken)
    {
        if (walletId <= 0)
        {
            return BadRequest("walletId must be greater than zero.");
        }

        var result = await _walletGatewayService.GetWalletTransactionsByWalletIdAsync(walletId, cancellationToken);

        return Ok(result);
    }
}
