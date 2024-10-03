using Azure.Messaging.EventHubs;
using Jobee.AuditTrail.Functions.Consumers;
using MassTransit;
using Microsoft.Azure.Functions.Worker;

namespace Jobee.AuditTrail.Functions.Functions;

public class EventLogMessageFunction
{
    private const string HubName = "event-logs";

    private readonly IEventReceiver _eventReceiver;

    public EventLogMessageFunction(IEventReceiver eventReceiver)
    {
        _eventReceiver = eventReceiver;
    }

    [Function("EventLogEventHubMessageFunction")]
    public async Task RunEventHubAsync([EventHubTrigger("event-logs", Connection = "")] EventData[] events,
        FunctionContext context)
    {
        foreach (var eventData in events)
        {
            await _eventReceiver.HandleConsumer<EventLogConsumer>(HubName, eventData, context.CancellationToken);
        }
    }
    
    [Function("AuditLogKafkaMessageFunction")]
    public async Task RunKafkaAsync([KafkaTrigger("", HubName)] EventData[] events,
        FunctionContext context)
    {
        foreach (var eventData in events)
        {
            await _eventReceiver.HandleConsumer<EventLogConsumer>(HubName, eventData, context.CancellationToken);
        }
    }
}