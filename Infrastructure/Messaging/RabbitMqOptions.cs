namespace Infrastructure.Messaging;

/// <summary>
/// Holds RabbitMQ broker and routing settings used by the Gateway outbox publisher for integration event
/// delivery.
/// </summary>
public class RabbitMqOptions
{
    public string HostName { get; set; } = string.Empty;

    public int Port { get; set; }

    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string VirtualHost { get; set; } = string.Empty;

    public string CreateOrderExchangeName { get; set; } = string.Empty;

    public string CreateOrderRoutingKey { get; set; } = string.Empty;
}
