namespace Infrastructure.Grpc;

/// <summary>
/// Holds the OrderService gRPC endpoint used by Gateway infrastructure services for inter-service wallet
/// queries.
/// </summary>
public class OrderServiceGrpcOptions
{
    public string GrpcAddress { get; set; } = string.Empty;
}
