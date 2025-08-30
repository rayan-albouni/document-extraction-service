using Azure.Messaging.ServiceBus;
using DocumentExtractionService.Domain.Interfaces;
using DocumentExtractionService.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DocumentExtractionService.Infrastructure.Services;

public class ServiceBusMessagePublisher : IMessagePublisher
{
    private readonly ServiceBusClient _client;
    private readonly ILogger<ServiceBusMessagePublisher> _logger;

    public ServiceBusMessagePublisher(
        IOptions<ServiceBusOptions> options,
        ILogger<ServiceBusMessagePublisher> logger)
    {
        var config = options.Value;
        _client = new ServiceBusClient(config.ConnectionString);
        _logger = logger;
    }

    public async Task PublishAsync<T>(T message, string queueName, CancellationToken cancellationToken = default) where T : class
    {
        _logger.LogInformation("Publishing message to queue: {QueueName}", queueName);

        await using var sender = _client.CreateSender(queueName);
        
        var messageBody = JsonConvert.SerializeObject(message);
        var serviceBusMessage = new ServiceBusMessage(messageBody);
        
        await sender.SendMessageAsync(serviceBusMessage, cancellationToken);
        
        _logger.LogInformation("Successfully published message to queue: {QueueName}", queueName);
    }
}
