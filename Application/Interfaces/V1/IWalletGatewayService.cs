using Contracts.Grpc.Models;

namespace Application.Interfaces.V1;

/// <summary>
/// Defines Gateway operations for wallet queries that are served through gRPC inter-service communication
/// with the OrderService.
/// </summary>
public interface IWalletGatewayService
{
    /// <summary>
    /// Retrieves the wallet associated with a customer through the OrderService wallet gRPC contract.
    /// </summary>
    Task<GetWalletByCustomerIdGrpcResponse> GetWalletByCustomerIdAsync(
        long customerId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves wallet transactions through the OrderService wallet gRPC contract.
    /// </summary>
    Task<GetWalletTransactionsByWalletIdGrpcResponse> GetWalletTransactionsByWalletIdAsync(
        long walletId,
        CancellationToken cancellationToken);
}
