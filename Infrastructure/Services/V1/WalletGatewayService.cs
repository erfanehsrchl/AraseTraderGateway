using Application.Interfaces.V1;
using Contracts.Grpc.Models;
using Contracts.Grpc.Wallet;

namespace Infrastructure.Services.V1;

/// <summary>
/// Implements wallet read operations using the OrderService protobuf-net.Grpc client supplied by dependency
/// injection, keeping controllers independent from direct gRPC infrastructure concerns.
/// </summary>
public class WalletGatewayService : IWalletGatewayService
{
    private readonly IWalletGrpcService _walletGrpcService;

    public WalletGatewayService(IWalletGrpcService walletGrpcService)
    {
        _walletGrpcService = walletGrpcService;
    }

    public async Task<GetWalletByCustomerIdGrpcResponse> GetWalletByCustomerIdAsync(
        long customerId,
        CancellationToken cancellationToken)
    {
        return await _walletGrpcService.GetWalletByCustomerIdAsync(
            new GetWalletByCustomerIdGrpcRequest
            {
                CustomerId = customerId
            },
            cancellationToken);
    }

    public async Task<GetWalletTransactionsByWalletIdGrpcResponse> GetWalletTransactionsByWalletIdAsync(
        long walletId,
        CancellationToken cancellationToken)
    {
        return await _walletGrpcService.GetWalletTransactionsByWalletIdAsync(
            new GetWalletTransactionsByWalletIdGrpcRequest
            {
                WalletId = walletId
            },
            cancellationToken);
    }
}
