using DocumentExtractionService.Application.Handlers;
using DocumentExtractionService.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace DocumentExtractionService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IDocumentExtractionHandler, DocumentExtractionHandler>();
        
        return services;
    }
}
