using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using DocumentExtractionService.Domain.Entities;
using DocumentExtractionService.Domain.Enums;
using DocumentExtractionService.Domain.Interfaces;
using DocumentExtractionService.Domain.ValueObjects;
using DocumentExtractionService.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace DocumentExtractionService.Infrastructure.Services;

public class DocumentIntelligenceService : IDocumentIntelligenceService
{
    private readonly DocumentAnalysisClient _client;
    private readonly ILogger<DocumentIntelligenceService> _logger;

    public DocumentIntelligenceService(
        IOptions<DocumentIntelligenceOptions> options,
        ILogger<DocumentIntelligenceService> logger)
    {
        var config = options.Value;
        _client = new DocumentAnalysisClient(new Uri(config.Endpoint), new AzureKeyCredential(config.ApiKey));
        _logger = logger;
    }

    public async Task<DocumentExtractedMessage> ExtractDocumentDataAsync(DocumentExtractionRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting document analysis for DocumentId: {DocumentId}, DocumentType: {DocumentType}",
            request.DocumentId, request.DocumentType);

        var modelConfig = GetModelConfiguration(request.DocumentType);

        var operation = await _client.AnalyzeDocumentFromUriAsync(
            WaitUntil.Completed,
            modelConfig.ModelId,
            new Uri(request.BlobUrl),
            cancellationToken: cancellationToken);

        var result = operation.Value;
        var parsedData = ConvertToJson(result);

        _logger.LogInformation("Successfully analyzed document for DocumentId: {DocumentId}", request.DocumentId);

        return new DocumentExtractedMessage
        {
            DocumentId = request.DocumentId,
            TenantId = request.TenantId,
            DocumentType = request.DocumentType,
            ParsedData = parsedData
        };
    }

    private static ModelConfiguration GetModelConfiguration(DocumentType documentType)
    {
        if (ModelConfiguration.Configurations.TryGetValue(documentType, out var config))
        {
            return config;
        }

        throw new NotSupportedException($"Document type {documentType} is not supported");
    }

    private static JObject ConvertToJson(AnalyzeResult result)
    {
        var json = new JObject
        {
            ["documents"] = new JArray(result.Documents.Select(ConvertDocument)),
            ["pages"] = new JArray(result.Pages.Select(ConvertPage)),
            ["tables"] = new JArray(result.Tables.Select(ConvertTable)),
            ["keyValuePairs"] = new JArray(result.KeyValuePairs.Select(ConvertKeyValuePair))
        };

        return json;
    }

    private static JObject ConvertDocument(AnalyzedDocument document)
    {
        var docObj = new JObject
        {
            ["docType"] = document.DocumentType,
            ["confidence"] = document.Confidence,
            ["fields"] = new JObject()
        };

        foreach (var field in document.Fields)
        {
            docObj["fields"]![field.Key] = ConvertDocumentField(field.Value);
        }

        return docObj;
    }

    private static JToken ConvertDocumentField(DocumentField field)
    {
        var fieldObj = new JObject
        {
            ["fieldType"] = field.FieldType.ToString(),
            ["confidence"] = field.Confidence
        };

        fieldObj["value"] = field.FieldType switch
        {
            DocumentFieldType.String => field.Value.AsString(),
            DocumentFieldType.Date => field.Value.AsDate().ToString("yyyy-MM-dd"),
            DocumentFieldType.Time => field.Value.AsTime().ToString(),
            DocumentFieldType.PhoneNumber => field.Value.AsPhoneNumber(),
            DocumentFieldType.Double => field.Value.AsDouble(),
            DocumentFieldType.Int64 => field.Value.AsInt64(),
            DocumentFieldType.List => new JArray(field.Value.AsList().Select(ConvertDocumentField)),
            DocumentFieldType.Dictionary => ConvertDictionary(field.Value.AsDictionary()),
            DocumentFieldType.Currency => ConvertCurrency(field.Value.AsCurrency()),
            DocumentFieldType.Address => ConvertAddress(field.Value.AsAddress()),
            _ => field.Content
        };

        return fieldObj;
    }

    private static JObject ConvertDictionary(IReadOnlyDictionary<string, DocumentField> dictionary)
    {
        var obj = new JObject();
        foreach (var kvp in dictionary)
        {
            obj[kvp.Key] = ConvertDocumentField(kvp.Value);
        }
        return obj;
    }

    private static JObject ConvertCurrency(CurrencyValue currency)
    {
        return new JObject
        {
            ["amount"] = currency.Amount,
            ["currencySymbol"] = currency.Symbol,
            ["currencyCode"] = currency.Code
        };
    }

    private static JObject ConvertAddress(AddressValue address)
    {
        return new JObject
        {
            ["houseNumber"] = address.HouseNumber,
            ["road"] = address.Road,
            ["city"] = address.City,
            ["state"] = address.State,
            ["postalCode"] = address.PostalCode,
            ["countryRegion"] = address.CountryRegion,
            ["streetAddress"] = address.StreetAddress
        };
    }

    private static JObject ConvertPage(DocumentPage page)
    {
        return new JObject
        {
            ["pageNumber"] = page.PageNumber,
            ["angle"] = page.Angle,
            ["width"] = page.Width,
            ["height"] = page.Height,
            ["unit"] = page.Unit.ToString(),
            ["lines"] = new JArray(page.Lines.Select(line => new JObject
            {
                ["content"] = line.Content,
                ["polygon"] = new JArray(line.BoundingPolygon.Select(point => new JArray { point.X, point.Y }))
            }))
        };
    }

    private static JObject ConvertTable(DocumentTable table)
    {
        return new JObject
        {
            ["rowCount"] = table.RowCount,
            ["columnCount"] = table.ColumnCount,
            ["cells"] = new JArray(table.Cells.Select(cell => new JObject
            {
                ["content"] = cell.Content,
                ["rowIndex"] = cell.RowIndex,
                ["columnIndex"] = cell.ColumnIndex,
                ["rowSpan"] = cell.RowSpan,
                ["columnSpan"] = cell.ColumnSpan
            }))
        };
    }

    private static JObject ConvertKeyValuePair(DocumentKeyValuePair kvp)
    {
        return new JObject
        {
            ["key"] = kvp.Key?.Content,
            ["value"] = kvp.Value?.Content,
            ["confidence"] = kvp.Confidence
        };
    }
}
