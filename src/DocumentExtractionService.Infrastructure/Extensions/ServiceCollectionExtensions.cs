using DocumentExtractionService.Domain.Interfaces;
using DocumentExtractionService.Infrastructure.Configuration;
using DocumentExtractionService.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentExtractionService.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DocumentIntelligenceOptions>(
            configuration.GetSection(DocumentIntelligenceOptions.SectionName));
        
        services.Configure<ServiceBusOptions>(
            configuration.GetSection(ServiceBusOptions.SectionName));

        services.AddScoped<IDocumentIntelligenceService, DocumentIntelligenceService>();
        services.AddScoped<IMessagePublisher, ServiceBusMessagePublisher>();

        return services;
    }
}
