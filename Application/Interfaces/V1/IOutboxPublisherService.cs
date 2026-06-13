namespace Application.Interfaces.V1;

/// <summary>
/// Defines the application boundary for publishing pending Gateway outbox messages to the message broker.
/// </summary>
public interface IOutboxPublisherService
{
    /// <summary>
    /// Publishes pending outbox messages and records delivery attempts for reliable message delivery.
    /// </summary>
    Task PublishPendingMessagesAsync(CancellationToken cancellationToken);
}
