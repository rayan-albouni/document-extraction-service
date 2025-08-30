using DocumentExtractionService.Domain.Entities;

namespace DocumentExtractionService.Domain.Interfaces;

public interface IDocumentIntelligenceService
{
    Task<DocumentExtractedMessage> ExtractDocumentDataAsync(DocumentExtractionRequest request, CancellationToken cancellationToken = default);
}
