using Application.Interfaces.V1;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.V1;

/// <summary>
/// Processes pending Gateway outbox messages and updates delivery state for successful, retriable, and failed
/// publishing attempts without depending on a specific message broker.
/// </summary>
public class OutboxPublisherService : IOutboxPublisherService
{
    private const int BatchSize = 100;
    private const int MaxRetryCount = 5;

    private readonly GatewayDbContext _dbContext;
    private readonly IMessageBusPublisher _messageBusPublisher;

    public OutboxPublisherService(
        GatewayDbContext dbContext,
        IMessageBusPublisher messageBusPublisher)
    {
        _dbContext = dbContext;
        _messageBusPublisher = messageBusPublisher;
    }

    public async Task PublishPendingMessagesAsync(CancellationToken cancellationToken)
    {
        var messages = await _dbContext.OutboxMessages
            .Where(message => message.Status == OutboxMessageStatus.Pending)
            .OrderBy(message => message.CreatedAt)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var attemptAt = DateTime.UtcNow;

            try
            {
                await _messageBusPublisher.PublishAsync(message, cancellationToken);

                message.Status = OutboxMessageStatus.Published;
                message.PublishedAt = attemptAt;
                message.LastAttemptAt = attemptAt;
                message.Error = null;
            }
            catch (Exception exception)
            {
                message.RetryCount += 1;
                message.LastAttemptAt = attemptAt;
                message.Error = exception.Message;

                if (message.RetryCount >= MaxRetryCount)
                {
                    message.Status = OutboxMessageStatus.Failed;
                }
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
