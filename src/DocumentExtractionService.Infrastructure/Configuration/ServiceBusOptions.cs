namespace DocumentExtractionService.Infrastructure.Configuration;

public class ServiceBusOptions
{
    public const string SectionName = "ServiceBus";
    
    public required string ConnectionString { get; set; }
}
