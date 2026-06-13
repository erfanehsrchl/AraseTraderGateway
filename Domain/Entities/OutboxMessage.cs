using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents a durable integration message stored by the Gateway before it is published to RabbitMQ.
/// This entity is the persistence boundary for the Outbox Pattern and allows order requests to be accepted
/// without requiring immediate broker availability.
/// </summary>
public class OutboxMessage
{
    public long Id { get; set; }

    public Guid MessageId { get; set; }

    public string MessageType { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public string? IdempotencyKey { get; set; }

    public OutboxMessageStatus Status { get; set; }

    public int RetryCount { get; set; }

    public string? Error { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastAttemptAt { get; set; }

    public DateTime? PublishedAt { get; set; }
}
