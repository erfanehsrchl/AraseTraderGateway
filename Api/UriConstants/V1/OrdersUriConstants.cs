namespace Api.UriConstants.V1;

public static class OrdersUriConstants
{
    public const string Route = "api/v{version:apiVersion}/orders";
    public const string GetByTrackingId = "{trackingId:guid}";
}
