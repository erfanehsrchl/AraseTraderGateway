using System.Text.Json;
using Application.DTOs;
using Application.Interfaces.V1;
using Contracts.Events;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Extensions;
using Infrastructure.Persistence;

namespace Infrastructure.Services.V1;

/// <summary>
/// Implements the order intake use case for the Gateway by creating a tracking identifier and storing a
/// CreateOrder integration event in the outbox for later RabbitMQ delivery.
/// </summary>
public class OrderGatewayService : IOrderGatewayService
{
    private readonly GatewayDbContext _dbContext;

    public OrderGatewayService(GatewayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AddOrderOutDto> AddOrderAsync(
        AddOrderInDto input,
        CancellationToken cancellationToken)
    {
        var trackingId = Guid.Empty.CreateVersion7();
        var now = DateTime.UtcNow;

        var createOrderEvent = new CreateOrderEvent
        {
            TrackingId = trackingId,
            CustomerId = input.CustomerId,
            Side = input.Side,
            Amount = input.Amount,
            OccurredAt = now
        };

        var outboxMessage = new OutboxMessage
        {
            MessageId = Guid.Empty.CreateVersion7(),
            MessageType = nameof(CreateOrderEvent),
            Payload = JsonSerializer.Serialize(createOrderEvent),
            Status = OutboxMessageStatus.Pending,
            RetryCount = 0,
            CreatedAt = now
        };

        _dbContext.OutboxMessages.Add(outboxMessage);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new AddOrderOutDto
        {
            TrackingId = trackingId
        };
    }
}
