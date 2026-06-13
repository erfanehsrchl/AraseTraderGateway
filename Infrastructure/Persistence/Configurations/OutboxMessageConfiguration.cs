using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(outboxMessage => outboxMessage.Id);

        builder.Property(outboxMessage => outboxMessage.MessageId)
            .IsRequired();

        builder.Property(outboxMessage => outboxMessage.MessageType)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(outboxMessage => outboxMessage.Payload)
            .IsRequired();

        builder.Property(outboxMessage => outboxMessage.IdempotencyKey)
            .HasMaxLength(100);

        builder.Property(outboxMessage => outboxMessage.Status)
            .IsRequired();

        builder.Property(outboxMessage => outboxMessage.RetryCount)
            .IsRequired();

        builder.Property(outboxMessage => outboxMessage.Error)
            .HasMaxLength(2000);

        builder.Property(outboxMessage => outboxMessage.CreatedAt);

        builder.Property(outboxMessage => outboxMessage.LastAttemptAt);

        builder.Property(outboxMessage => outboxMessage.PublishedAt);

        builder.HasIndex(outboxMessage => outboxMessage.MessageId);

        builder.HasIndex(outboxMessage => outboxMessage.IdempotencyKey)
            .IsUnique()
            .HasFilter("\"IdempotencyKey\" IS NOT NULL");

        builder.HasIndex(outboxMessage => new
        {
            outboxMessage.Status,
            outboxMessage.CreatedAt
        });
    }
}
