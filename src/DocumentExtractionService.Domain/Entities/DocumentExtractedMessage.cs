using Newtonsoft.Json.Linq;

namespace DocumentExtractionService.Domain.Entities;

public record DocumentExtractedMessage
{
    public Guid DocumentId { get; set; }
    public required string TenantId { get; set; }
    public JObject ParsedData { get; set; } = new();
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}
