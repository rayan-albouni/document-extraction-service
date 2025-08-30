using DocumentExtractionService.Application.Interfaces;
using DocumentExtractionService.Domain.Entities;
using DocumentExtractionService.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace DocumentExtractionService.Application.Handlers;

public class DocumentExtractionHandler : IDocumentExtractionHandler
{
    private readonly IDocumentIntelligenceService _documentIntelligenceService;
    private readonly IMessagePublisher _messagePublisher;
    private readonly ILogger<DocumentExtractionHandler> _logger;

    public DocumentExtractionHandler(
        IDocumentIntelligenceService documentIntelligenceService,
        IMessagePublisher messagePublisher,
        ILogger<DocumentExtractionHandler> logger)
    {
        _documentIntelligenceService = documentIntelligenceService;
        _messagePublisher = messagePublisher;
        _logger = logger;
    }

    public async Task HandleAsync(DocumentExtractionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing document extraction for DocumentId: {DocumentId}, TenantId: {TenantId}, DocumentType: {DocumentType}", 
            request.DocumentId, request.TenantId, request.DocumentType);

        try
        {
            var extractedResult = await _documentIntelligenceService.ExtractDocumentDataAsync(request, cancellationToken);
            
            await _messagePublisher.PublishAsync(extractedResult, "document-extraction-results", cancellationToken);
            
            _logger.LogInformation("Successfully processed document extraction for DocumentId: {DocumentId}", request.DocumentId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process document extraction for DocumentId: {DocumentId}", request.DocumentId);
            throw;
        }
    }
}
