using DocumentExtractionService.Domain.Enums;

namespace DocumentExtractionService.Domain.Entities;

public class DocumentExtractionRequest
{
    public Guid DocumentId { get; set; }
    public required string TenantId { get; set; }
    public DocumentType DocumentType { get; set; }
    public required string BlobUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
