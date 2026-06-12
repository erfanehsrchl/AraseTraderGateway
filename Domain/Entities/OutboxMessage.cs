using Domain.Enums;

namespace Domain.Entities;

public class OutboxMessage
{
    public long Id { get; set; }

    public Guid MessageId { get; set; }

    public string MessageType { get; set; } = string.Empty;

    public string Payload { get; set; } = string.Empty;

    public OutboxMessageStatus Status { get; set; }

    public int RetryCount { get; set; }

    public string? Error { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? LastAttemptAt { get; set; }

    public DateTime? PublishedAt { get; set; }
}
