using System.Text.Json;
using Application.DTOs;
using Application.Interfaces.V1;
using Contracts.Events;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Infrastructure.Services.V1;

/// <summary>
/// Implements the order intake use case for the Gateway by creating a tracking identifier and storing a
/// CreateOrder integration event in the outbox for later RabbitMQ delivery.
/// </summary>
public class OrderGatewayService : IOrderGatewayService
{
    private const string UniqueViolationSqlState = "23505";

    private readonly GatewayDbContext _dbContext;

    public OrderGatewayService(GatewayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AddOrderOutDto> AddOrderAsync(
        AddOrderInDto input,
        CancellationToken cancellationToken)
    {
        var existingOutboxMessage = await GetExistingOutboxMessageAsync(input.IdempotencyKey, nameof(CreateOrderEvent), cancellationToken);

        if (existingOutboxMessage is not null)
        {
            return CreateOutputFromExistingMessage(existingOutboxMessage);
        }

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
            IdempotencyKey = input.IdempotencyKey,
            MessageId = Guid.Empty.CreateVersion7(),
            MessageType = nameof(CreateOrderEvent),
            Payload = JsonSerializer.Serialize(createOrderEvent),
            Status = OutboxMessageStatus.Pending,
            RetryCount = 0,
            CreatedAt = now
        };

        _dbContext.OutboxMessages.Add(outboxMessage);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsIdempotencyKeyUniqueViolation(exception))
        {
            _dbContext.Entry(outboxMessage).State = EntityState.Detached;

            var persistedOutboxMessage = await GetExistingOutboxMessageAsync(input.IdempotencyKey, nameof(CreateOrderEvent), cancellationToken);

            if (persistedOutboxMessage is null)
            {
                throw;
            }

            return CreateOutputFromExistingMessage(persistedOutboxMessage);
        }

        return new AddOrderOutDto
        {
            TrackingId = trackingId
        };
    }

    private async Task<OutboxMessage?> GetExistingOutboxMessageAsync(
        string idempotencyKey,
        string messageType,
        CancellationToken cancellationToken)
    {
        return await _dbContext.OutboxMessages
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x =>
                    x.IdempotencyKey == idempotencyKey &&
                    x.MessageType == messageType,
                cancellationToken);
    }

    private static AddOrderOutDto CreateOutputFromExistingMessage(OutboxMessage outboxMessage)
    {
        var createOrderEvent = JsonSerializer.Deserialize<CreateOrderEvent>(outboxMessage.Payload)
            ?? throw new InvalidOperationException("Outbox message payload could not be deserialized.");

        return new AddOrderOutDto
        {
            TrackingId = createOrderEvent.TrackingId
        };
    }

    private static bool IsIdempotencyKeyUniqueViolation(DbUpdateException exception)
    {
        return exception.InnerException is PostgresException postgresException
            && postgresException.SqlState == UniqueViolationSqlState
            && postgresException.ConstraintName == "IX_OutboxMessages_IdempotencyKey";
    }
}
