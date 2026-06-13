using Contracts.Grpc.Models;

namespace Application.Interfaces.V1;

/// <summary>
/// Defines Gateway order query operations that are served through gRPC inter-service communication
/// with the OrderService.
/// </summary>
public interface IOrderGrpcGatewayService
{
    /// <summary>
    /// Retrieves an order by its tracking identifier through the OrderService order gRPC contract.
    /// </summary>
    Task<GetOrderByTrackingIdGrpcResponse> GetOrderByTrackingIdAsync(
        Guid trackingId,
        CancellationToken cancellationToken);
}
