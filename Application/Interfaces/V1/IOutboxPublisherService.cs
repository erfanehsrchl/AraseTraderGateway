namespace Application.Interfaces.V1;

public interface IOutboxPublisherService
{
    Task PublishPendingMessagesAsync(CancellationToken cancellationToken);
}
