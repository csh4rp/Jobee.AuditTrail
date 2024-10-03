using Jobee.AuditTrail.Contracts.EventLogs;
using Jobee.AuditTrail.Domain.EventLogs;
using MassTransit;
using Microsoft.Azure.Cosmos;

namespace Jobee.AuditTrail.Functions.Consumers;

public class EventLogConsumer : IConsumer<Batch<EventLogIntegrationEvent>>
{
    private readonly CosmosClient _cosmosClient;

    public EventLogConsumer(CosmosClient cosmosClient) => _cosmosClient = cosmosClient;

    public async Task Consume(ConsumeContext<Batch<EventLogIntegrationEvent>> context)
    {
        var logGroups = context.Message.Select(e => new EventLog
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = e.Message.Source,
                Source = e.Message.Source,
                Timestamp = e.Message.Timestamp,
                Actor = e.Message.Actor,
                Subject = e.Message.Subject,
                Payload = e.Message.Payload
            })
        .GroupBy(l => l.Source);
        
        var container = _cosmosClient.GetContainer("AuditTrail", "event-logs");

        foreach (var group in logGroups)
        {
            var batch = container.CreateTransactionalBatch(new PartitionKey(group.Key));

            foreach (var log in group)
            {
                batch = batch.CreateItem(log);
            }

            using var response = await batch.ExecuteAsync(context.CancellationToken);
        }
    }
    
}