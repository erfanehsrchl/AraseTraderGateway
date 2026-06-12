using Api.ViewModels.V1;
using Application.DTOs;
using Mapster;

namespace Api.Mappings.V1;

public static class OrderMappingConfiguration
{
    public static void RegisterOrderMappings(this TypeAdapterConfig config)
    {
        config.NewConfig<AddOrderInVm, AddOrderInDto>();
        config.NewConfig<AddOrderOutDto, AddOrderOutVm>();
    }
}
