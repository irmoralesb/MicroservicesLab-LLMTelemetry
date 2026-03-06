using Azure.Data.Tables;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LLMTelemetry;

public class LLMTelemetryFuncion
{
    private readonly ILogger<LLMTelemetryFuncion> _logger;
    private readonly TableServiceClient _tableServiceClient;

    public LLMTelemetryFuncion(ILogger<LLMTelemetryFuncion> logger, TableServiceClient tableServiceClient)
    {
        _logger = logger;
        _tableServiceClient = tableServiceClient;
    }

    [Function(nameof(LLMTelemetryFuncion))]
    public async Task Run(
        [ServiceBusTrigger("llm-metric", "llm-telemetry", Connection = "SERVICEBUS_CONNSTR", IsSessionsEnabled = true)]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
    {
        _logger.LogInformation("Message ID: {id}", message.MessageId);
        _logger.LogInformation("Message Body: {body}", message.Body);
        _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

        try
        {
            var tableClient = _tableServiceClient.GetTableClient("llmmetricstable");
            await tableClient.CreateIfNotExistsAsync();
            var entity = new LLMTelemetryEntity
            {
                PartitionKey = DateTime.UtcNow.ToString("yyyyMMdd"),
                RowKey = Guid.NewGuid().ToString(),
                MessageId = message.MessageId,
                MessageBody = message.Body.ToString(),
                ContentType = message.ContentType
            };

            await tableClient.AddEntityAsync(entity);
            _logger.LogInformation("Successfully written to Table Storage. RowKey: {rowKey}", entity.RowKey);
            await messageActions.CompleteMessageAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error writing to Table Storage");
            throw;
        }
    }
}