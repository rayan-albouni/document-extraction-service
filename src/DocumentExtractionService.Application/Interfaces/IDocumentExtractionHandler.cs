using DocumentExtractionService.Domain.Entities;

namespace DocumentExtractionService.Application.Interfaces;

public interface IDocumentExtractionHandler
{
    Task HandleAsync(DocumentExtractionRequest request, CancellationToken cancellationToken = default);
}
