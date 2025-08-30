# DocumentExtractionService

A C# .NET 8.0 Azure Functions application that processes document extraction requests using Azure Document Intelligence and Service Bus messaging.

## Architecture

This project follows Domain-Driven Design (DDD) principles with a clean architecture approach:

- **Domain Layer**: Contains business entities, value objects, enums, and interfaces
- **Application Layer**: Contains application services and handlers
- **Infrastructure Layer**: Contains external service implementations (Azure Document Intelligence, Service Bus)
- **Functions Layer**: Contains Azure Functions as the entry point

## Project Structure

```
src/
├── DocumentExtractionService.sln
├── DocumentExtractionService.Domain/
│   ├── Entities/
│   ├── Enums/
│   ├── Interfaces/
│   └── ValueObjects/
├── DocumentExtractionService.Application/
│   ├── Extensions/
│   ├── Handlers/
│   └── Interfaces/
├── DocumentExtractionService.Infrastructure/
│   ├── Configuration/
│   ├── Extensions/
│   └── Services/
└── DocumentExtractionService.Functions/
    ├── Functions/
    ├── host.json
    ├── local.settings.json
    └── Program.cs
```

## Features

- **Message Processing**: Consumes messages from `document-extraction-queue` Service Bus queue
- **Document Intelligence**: Supports multiple document types (Invoice, Receipt, Business Card, etc.)
- **Extensible Models**: Easy to add new document types and models
- **Result Publishing**: Publishes extraction results to `document-extraction-results` queue
- **Logging**: Integrated with Application Insights for monitoring and diagnostics
- **Error Handling**: Comprehensive error handling with retry policies

## Supported Document Types

- Invoice (prebuilt-invoice)
- Receipt (prebuilt-receipt)  
- Business Card (prebuilt-businessCard)
- Identity Document (prebuilt-idDocument)
- Driver License (prebuilt-driverLicense)
- Credit Card (custom model support)
- Contract (custom model support)

## Configuration

Update the `local.settings.json` file with your Azure resource connection strings:

```json
{
  "ServiceBus": {
    "ConnectionString": "your-service-bus-connection-string"
  },
  "DocumentIntelligence": {
    "Endpoint": "your-document-intelligence-endpoint",
    "ApiKey": "your-document-intelligence-api-key"
  }
}
```

## Message Format

### Input Message (document-extraction-queue)
```json
{
  "DocumentId": "c627af3d-a95e-4273-989f-e0c4e8844ac4",
  "TenantId": "TenantIdtest",
  "DocumentType": "Invoice",
  "BlobUrl": "https://parseddocs.blob.core.windows.net/raw-documents.pdf"
}
```

### Output Message (document-extraction-results)
```json
{
  "DocumentId": "c627af3d-a95e-4273-989f-e0c4e8844ac4",
  "TenantId": "TenantIdtest",
  "ParsedData": { /* extracted document data */ },
  "ProcessedAt": "2024-01-15T10:30:00Z"
}
```

## Development

### Prerequisites
- .NET 8.0 SDK
- Azure Functions Core Tools
- Azure Storage Emulator or Azurite

### Running Locally
1. Update `local.settings.json` with your connection strings
2. Run `func start` in the Functions project directory
3. The function will listen for messages on the configured Service Bus queue

### Building
```bash
dotnet build src/DocumentExtractionService.sln
```

### Running Tests
```bash
dotnet test src/DocumentExtractionService.sln
```

## Deployment

The application is designed to be deployed as an Azure Function App with the following required Azure resources:

- Azure Function App
- Azure Service Bus (with queues: `document-extraction-queue` and `document-extraction-results`)
- Azure Document Intelligence (Cognitive Services)
- Application Insights
- Azure Storage Account

## Design Principles

This project adheres to:
- **SOLID Principles**: Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **DRY (Don't Repeat Yourself)**: Minimal code duplication
- **KISS (Keep It Simple, Stupid)**: Simple, readable code
- **Separation of Concerns**: Clear layer separation
- **Domain-Driven Design**: Business logic in the domain layer
- **Dependency Injection**: Loose coupling between components
- **High Performance**: Optimized for Azure Functions runtime
