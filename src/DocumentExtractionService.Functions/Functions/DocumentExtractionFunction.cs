using DocumentExtractionService.Application.Interfaces;
using DocumentExtractionService.Domain.Entities;
using DocumentExtractionService.Domain.Enums;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DocumentExtractionService.Functions.Functions;

public class DocumentExtractionFunction
{
    private readonly IDocumentExtractionHandler _handler;
    private readonly ILogger<DocumentExtractionFunction> _logger;

    public DocumentExtractionFunction(
        IDocumentExtractionHandler handler,
        ILogger<DocumentExtractionFunction> logger)
    {
        _handler = handler;
        _logger = logger;
    }

    [Function("ProcessDocumentExtraction")]
    public async Task ProcessDocumentExtraction(
        [ServiceBusTrigger("document-extraction-queue", Connection = "ServiceBus:ConnectionString")]
        string messageBody,
        FunctionContext context,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received document extraction message: {MessageBody}", messageBody);

        try
        {
            var request = ParseMessage(messageBody);
            await _handler.HandleAsync(request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process document extraction message: {MessageBody}", messageBody);
            throw;
        }
    }

    private static DocumentExtractionRequest ParseMessage(string messageBody)
    {
        var messageData = JsonConvert.DeserializeObject<dynamic>(messageBody)
            ?? throw new ArgumentException("Invalid message format");

        if (!Guid.TryParse(messageData.DocumentId?.ToString(), out Guid documentId))
            throw new ArgumentException("Invalid DocumentId format");

        var tenantId = messageData.TenantId?.ToString()
            ?? throw new ArgumentException("TenantId is required");

        var blobUrl = messageData.BlobUrl?.ToString()
            ?? throw new ArgumentException("BlobUrl is required");

        var documentTypeString = messageData.DocumentType?.ToString()
            ?? throw new ArgumentException("DocumentType is required");

        if (!Enum.TryParse<DocumentType>(documentTypeString, true, out DocumentType documentType))
            throw new ArgumentException($"Invalid DocumentType: {documentTypeString}");

        return new DocumentExtractionRequest
        {
            DocumentId = documentId,
            TenantId = tenantId,
            DocumentType = documentType,
            BlobUrl = blobUrl
        };
    }
}
