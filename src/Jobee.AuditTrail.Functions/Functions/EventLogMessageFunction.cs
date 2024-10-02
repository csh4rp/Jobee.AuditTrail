using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.EventHubs;
using Jobee.AuditTrail.Contracts.EventLogs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using EventLog = Jobee.AuditTrail.Domain.EventLogs.EventLog;

namespace Jobee.AuditTrail.Functions.Functions;

public class EventLogMessageFunction
{
    private readonly ILogger<EventLogMessageFunction> _logger;

    public EventLogMessageFunction(ILogger<EventLogMessageFunction> logger)
    {
        _logger = logger;
    }

    [Function(nameof(EventLogMessageFunction))]
    [CosmosDBOutput("AuditTrail", "event-logs")]
    public object? Run([EventHubTrigger("event-logs", Connection = "")] EventData[] events)
    {
        var eventLogs = new List<EventLog>(events.Length);
        
        foreach (var eventData in events)
        {
            var @event = JsonSerializer.Deserialize<EventLogIntegrationEvent>(eventData.BodyAsStream);
            
            Debug.Assert(@event is not null);

            var eventLog = new EventLog
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = @event.Source,
                Source = @event.Source,
                Timestamp = @event.Timestamp,
                Actor = @event.Actor,
                Subject = @event.Subject,
                Payload = @event.Payload
            };

            eventLogs.Add(eventLog);
        }
        
        return eventLogs.Count != 0 ? eventLogs : null;
    }
}