namespace DocumentExtractionService.Infrastructure.Configuration;

public class DocumentIntelligenceOptions
{
    public const string SectionName = "DocumentIntelligence";
    
    public required string Endpoint { get; set; }
    public required string ApiKey { get; set; }
}
