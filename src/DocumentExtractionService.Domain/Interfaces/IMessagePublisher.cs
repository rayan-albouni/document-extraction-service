namespace DocumentExtractionService.Domain.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class;
}
