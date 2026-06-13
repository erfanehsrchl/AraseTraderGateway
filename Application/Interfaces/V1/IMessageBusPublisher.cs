using Domain.Entities;

namespace Application.Interfaces.V1;

/// <summary>
/// Defines the infrastructure boundary for publishing integration events to an external message bus.
/// Outbox processing depends on this abstraction instead of a specific broker such as RabbitMQ.
/// </summary>
public interface IMessageBusPublisher
{
    /// <summary>
    /// Publishes a durable outbox message to the configured message bus while preserving its integration
    /// event metadata.
    /// </summary>
    Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken);
}
