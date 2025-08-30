using DocumentExtractionService.Domain.Enums;

namespace DocumentExtractionService.Domain.ValueObjects;

public record ModelConfiguration
{
    public DocumentType DocumentType { get; init; }
    public required string ModelId { get; init; }
    public required string ModelName { get; init; }
    public bool IsCustomModel { get; init; }
    
    public static readonly Dictionary<DocumentType, ModelConfiguration> Configurations = new()
    {
        {
            DocumentType.Invoice,
            new ModelConfiguration
            {
                DocumentType = DocumentType.Invoice,
                ModelId = "prebuilt-invoice",
                ModelName = "Prebuilt Invoice",
                IsCustomModel = false
            }
        },
        {
            DocumentType.Receipt,
            new ModelConfiguration
            {
                DocumentType = DocumentType.Receipt,
                ModelId = "prebuilt-receipt",
                ModelName = "Prebuilt Receipt",
                IsCustomModel = false
            }
        },
        {
            DocumentType.BusinessCard,
            new ModelConfiguration
            {
                DocumentType = DocumentType.BusinessCard,
                ModelId = "prebuilt-businessCard",
                ModelName = "Prebuilt Business Card",
                IsCustomModel = false
            }
        },
        {
            DocumentType.IdentityDocument,
            new ModelConfiguration
            {
                DocumentType = DocumentType.IdentityDocument,
                ModelId = "prebuilt-idDocument",
                ModelName = "Prebuilt Identity Document",
                IsCustomModel = false
            }
        },
        {
            DocumentType.DriverLicense,
            new ModelConfiguration
            {
                DocumentType = DocumentType.DriverLicense,
                ModelId = "prebuilt-driverLicense",
                ModelName = "Prebuilt Driver License",
                IsCustomModel = false
            }
        }
    };
}
