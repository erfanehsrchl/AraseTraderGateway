using System.Text;
using Application.Interfaces.V1;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Messaging;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure.Services.V1;

public class OutboxPublisherService : IOutboxPublisherService
{
    private const int BatchSize = 100;
    private const int MaxRetryCount = 5;

    private readonly GatewayDbContext _dbContext;
    private readonly RabbitMqOptions _rabbitMqOptions;

    public OutboxPublisherService(
        GatewayDbContext dbContext,
        IOptions<RabbitMqOptions> rabbitMqOptions)
    {
        _dbContext = dbContext;
        _rabbitMqOptions = rabbitMqOptions.Value;
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
                PublishMessage(message);

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

    private void PublishMessage(OutboxMessage message)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password,
            VirtualHost = _rabbitMqOptions.VirtualHost
        };

        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(
            exchange: _rabbitMqOptions.CreateOrderExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.MessageId = message.MessageId.ToString();
        properties.Type = message.MessageType;
        properties.Headers = new Dictionary<string, object>
        {
            ["message-type"] = message.MessageType
        };

        channel.BasicPublish(
            exchange: _rabbitMqOptions.CreateOrderExchangeName,
            routingKey: _rabbitMqOptions.CreateOrderRoutingKey,
            basicProperties: properties,
            body: Encoding.UTF8.GetBytes(message.Payload));
    }
}
