using DocumentExtractionService.Domain.Enums;
using Newtonsoft.Json;

namespace DocumentExtractionService.Domain.Entities;

public class DocumentExtractionRequest
{
    public Guid DocumentId { get; set; }
    public required string TenantId { get; set; }
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public DocumentType DocumentType { get; set; }
    public required string BlobUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
