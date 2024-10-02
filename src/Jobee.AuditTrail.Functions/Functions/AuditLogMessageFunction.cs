using System.Diagnostics;
using System.Text.Json;
using Azure.Messaging.EventHubs;
using Jobee.AuditTrail.Contracts.AuditLogs;
using Jobee.AuditTrail.Domain.AuditLogs;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Jobee.AuditTrail.Functions.Functions;

public class AuditLogMessageFunction
{
    private readonly ILogger<AuditLogMessageFunction> _logger;

    public AuditLogMessageFunction(ILogger<AuditLogMessageFunction> logger) => _logger = logger;

    [Function(nameof(AuditLogMessageFunction))]
    [CosmosDBOutput("AuditTrail", "audit-logs")]
    public object? Run([EventHubTrigger("audit-logs", Connection = "")] EventData[] events)
    {
        var auditLogs = new List<AuditLog>(events.Length);
        
        foreach (var eventData in events)
        {
            var @event = JsonSerializer.Deserialize<AuditLogIntegrationEvent>(eventData.BodyAsStream);
            
            Debug.Assert(@event is not null);

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = @event.Source,
                Source = @event.Source,
                Timestamp = @event.Timestamp,
                Actor = @event.Actor,
                ObjectName = @event.ObjectName,
                ObjectType = @event.ObjectType,
                ObjectStatus = Enum.Parse<ObjectStatus>(@event.ObjectStatus.ToString()),
                MetaData = @event.MetaData,
                Changes = @event.Changes.Select(c => new ObjectChange(c.FieldName, c.OldValue, c.NewValue)).ToArray()
            };

            auditLogs.Add(auditLog);
        }
        
        return auditLogs.Count != 0 ? auditLogs : null;
    }
}