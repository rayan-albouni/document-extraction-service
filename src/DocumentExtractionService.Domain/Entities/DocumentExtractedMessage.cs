using DocumentExtractionService.Domain.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DocumentExtractionService.Domain.Entities;

public record DocumentExtractedMessage
{
    public Guid DocumentId { get; set; }
    public required string TenantId { get; set; }
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public DocumentType DocumentType { get; set; }
    public JObject ParsedData { get; set; } = new();
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
