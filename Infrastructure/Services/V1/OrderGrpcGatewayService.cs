using Application.Interfaces.V1;
using Contracts.Grpc.Models;
using Contracts.Grpc.Order;

namespace Infrastructure.Services.V1;

/// <summary>
/// Implements order query operations using the OrderService protobuf-net.Grpc client supplied by dependency
/// injection, keeping controllers independent from direct gRPC infrastructure concerns.
/// </summary>
public class OrderGrpcGatewayService : IOrderGrpcGatewayService
{
    private readonly IOrderGrpcService _orderGrpcService;

    public OrderGrpcGatewayService(IOrderGrpcService orderGrpcService)
    {
        _orderGrpcService = orderGrpcService;
    }

    public async Task<GetOrderByTrackingIdGrpcResponse> GetOrderByTrackingIdAsync(
        Guid trackingId,
        CancellationToken cancellationToken)
    {
        return await _orderGrpcService.GetOrderByTrackingIdAsync(
            new GetOrderByTrackingIdGrpcRequest
            {
                TrackingId = trackingId
            },
            cancellationToken);
    }
}
