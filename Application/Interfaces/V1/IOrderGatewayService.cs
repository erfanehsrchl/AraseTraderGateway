using Application.DTOs;

namespace Application.Interfaces.V1;

/// <summary>
/// Defines the Gateway use case for accepting order requests and translating them into reliable outbound
/// integration messages.
/// </summary>
public interface IOrderGatewayService
{
    /// <summary>
    /// Creates a trackable order request in the Gateway and stages the corresponding integration event
    /// through the Outbox Pattern.
    /// </summary>
    Task<AddOrderOutDto> AddOrderAsync(
        AddOrderInDto input,
        CancellationToken cancellationToken);
}
