using Application.Interfaces.V1;
using Contracts.Grpc.Models;
using Contracts.Grpc.Wallet;
using Grpc.Net.Client;
using Infrastructure.Grpc;
using Microsoft.Extensions.Options;
using ProtoBuf.Grpc.Client;

namespace Infrastructure.Services.V1;

public class WalletGatewayService : IWalletGatewayService
{
    private readonly OrderServiceGrpcOptions _orderServiceGrpcOptions;

    public WalletGatewayService(IOptions<OrderServiceGrpcOptions> orderServiceGrpcOptions)
    {
        _orderServiceGrpcOptions = orderServiceGrpcOptions.Value;
    }

    public async Task<GetWalletByCustomerIdGrpcResponse> GetWalletByCustomerIdAsync(
        long customerId,
        CancellationToken cancellationToken)
    {
        using var channel = GrpcChannel.ForAddress(_orderServiceGrpcOptions.GrpcAddress);
        var client = channel.CreateGrpcService<IWalletGrpcService>();

        return await client.GetWalletByCustomerIdAsync(
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
        using var channel = GrpcChannel.ForAddress(_orderServiceGrpcOptions.GrpcAddress);
        var client = channel.CreateGrpcService<IWalletGrpcService>();

        return await client.GetWalletTransactionsByWalletIdAsync(
            new GetWalletTransactionsByWalletIdGrpcRequest
            {
                WalletId = walletId
            },
            cancellationToken);
    }
}
