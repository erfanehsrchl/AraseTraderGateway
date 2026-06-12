using Application.DTOs;

namespace Application.Interfaces.V1;

public interface IOrderGatewayService
{
    Task<AddOrderOutDto> AddOrderAsync(
        AddOrderInDto input,
        CancellationToken cancellationToken);
}
