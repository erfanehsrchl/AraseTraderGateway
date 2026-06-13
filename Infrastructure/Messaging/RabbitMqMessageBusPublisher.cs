using System.Text;
using Application.Interfaces.V1;
using Domain.Entities;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Infrastructure.Messaging;

/// <summary>
/// RabbitMQ implementation of the message bus publisher used by the Gateway outbox pipeline.
/// The RabbitMQ connection is reused for the lifetime of the singleton while channels are created per publish.
/// </summary>
public sealed class RabbitMqMessageBusPublisher : IMessageBusPublisher, IDisposable
{
    private readonly object _connectionLock = new();
    private readonly RabbitMqOptions _rabbitMqOptions;
    private IConnection? _connection;

    public RabbitMqMessageBusPublisher(IOptions<RabbitMqOptions> rabbitMqOptions)
    {
        _rabbitMqOptions = rabbitMqOptions.Value;
    }

    public Task PublishAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var channel = GetConnection().CreateModel();

        channel.ExchangeDeclare(
            exchange: _rabbitMqOptions.CreateOrderExchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false);

        var properties = channel.CreateBasicProperties();
        properties.Persistent = true;
        properties.ContentType = "application/json";
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

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }

    private IConnection GetConnection()
    {
        if (_connection?.IsOpen == true)
        {
            return _connection;
        }

        lock (_connectionLock)
        {
            if (_connection?.IsOpen == true)
            {
                return _connection;
            }

            _connection?.Dispose();
            _connection = CreateConnection();

            return _connection;
        }
    }

    private IConnection CreateConnection()
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _rabbitMqOptions.HostName,
            Port = _rabbitMqOptions.Port,
            UserName = _rabbitMqOptions.UserName,
            Password = _rabbitMqOptions.Password,
            VirtualHost = _rabbitMqOptions.VirtualHost,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true
        };

        return connectionFactory.CreateConnection();
    }
}
