using Contracts.Grpc.Models;

namespace Application.Interfaces.V1;

public interface IWalletGatewayService
{
    Task<GetWalletByCustomerIdGrpcResponse> GetWalletByCustomerIdAsync(
        long customerId,
        CancellationToken cancellationToken);

    Task<GetWalletTransactionsByWalletIdGrpcResponse> GetWalletTransactionsByWalletIdAsync(
        long walletId,
        CancellationToken cancellationToken);
}
