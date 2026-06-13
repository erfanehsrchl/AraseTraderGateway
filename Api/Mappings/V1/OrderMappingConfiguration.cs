using Api.ViewModels.V1;
using Application.DTOs;
using Contracts.Grpc.Models;
using Mapster;

namespace Api.Mappings.V1;

public static class OrderMappingConfiguration
{
    public static void RegisterOrderMappings(this TypeAdapterConfig config)
    {
        config.NewConfig<AddOrderInVm, AddOrderInDto>();
        config.NewConfig<AddOrderOutDto, AddOrderOutVm>();
        config.NewConfig<GetOrderByTrackingIdGrpcResponse, GetOrderByTrackingIdOutVm>();
        config.NewConfig<GetWalletByCustomerIdGrpcResponse, GetWalletByCustomerIdOutVm>();
        config.NewConfig<WalletTransactionGrpcDto, WalletTransactionOutVm>();
        config.NewConfig<GetWalletTransactionsByWalletIdGrpcResponse, GetWalletTransactionsByWalletIdOutVm>();
    }
}
