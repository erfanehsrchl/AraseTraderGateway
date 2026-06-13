using Contracts.Enums;

namespace Api.ViewModels.V1;

public class GetOrderByTrackingIdOutVm
{
    public Guid TrackingId { get; set; }

    public long CustomerId { get; set; }

    public OrderSideContract Side { get; set; }

    public decimal Amount { get; set; }

    public OrderStatusContract Status { get; set; }

    public string? FailureReason { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
