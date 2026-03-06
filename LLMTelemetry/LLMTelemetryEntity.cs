using Azure;
using Azure.Data.Tables;

namespace LLMTelemetry;

public class LLMTelemetryEntity : ITableEntity
{
    public string PartitionKey { get; set; } = default!;
    public string RowKey { get; set; } = default!;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    
    public string? MessageId { get; set; }
    public string? MessageBody { get; set; }
    public string? ContentType { get; set; }
}