using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace LLMTelemetry;

public class LLMTelemetryFuncion
{
    private readonly ILogger<LLMTelemetryFuncion> _logger;

    public LLMTelemetryFuncion(ILogger<LLMTelemetryFuncion> logger)
    {
        _logger = logger;
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

            // Complete the message
        await messageActions.CompleteMessageAsync(message);
    }
}