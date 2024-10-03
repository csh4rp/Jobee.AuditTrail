using Azure.Messaging.EventHubs;
using Jobee.AuditTrail.Functions.Consumers;
using MassTransit;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Jobee.AuditTrail.Functions.Functions;

public class AuditLogMessageFunction
{
    private const string HubName = "audit-logs";
    
    private readonly ILogger<AuditLogMessageFunction> _logger;
    private readonly IEventReceiver _eventReceiver;

    public AuditLogMessageFunction(ILogger<AuditLogMessageFunction> logger, IEventReceiver eventReceiver)
    {
        _logger = logger;
        _eventReceiver = eventReceiver;
    }

    [Function("AuditLogEventHubMessageFunction")]
    public async Task RunEventHubAsync([EventHubTrigger(HubName, Connection = "")] EventData[] events,
        FunctionContext context)
    {
        foreach (var eventData in events)
        {
            await _eventReceiver.HandleConsumer<AuditLogConsumer>(HubName, eventData, context.CancellationToken);
        }
    }
    
    [Function("AuditLogKafkaMessageFunction")]
    public async Task RunKafkaAsync([KafkaTrigger("", HubName)] EventData[] events,
        FunctionContext context)
    {
        foreach (var eventData in events)
        {
            await _eventReceiver.HandleConsumer<AuditLogConsumer>(HubName, eventData, context.CancellationToken);
        }
    }
}