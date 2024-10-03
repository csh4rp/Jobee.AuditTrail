using Jobee.AuditTrail.Contracts.AuditLogs;
using Jobee.AuditTrail.Domain.AuditLogs;
using MassTransit;
using Microsoft.Azure.Cosmos;

namespace Jobee.AuditTrail.Functions.Consumers;

public class AuditLogConsumer : IConsumer<Batch<AuditLogIntegrationEvent>>
{
    private readonly CosmosClient _cosmosClient;

    public AuditLogConsumer(CosmosClient cosmosClient) => _cosmosClient = cosmosClient;

    public async Task Consume(ConsumeContext<Batch<AuditLogIntegrationEvent>> context)
    {
        var logGroups = context.Message.Select(e => new AuditLog
            {
                Id = Guid.NewGuid().ToString(),
                PartitionKey = e.Message.Source,
                Source = e.Message.Source,
                Timestamp = e.Message.Timestamp,
                Actor = e.Message.Actor,
                ObjectName = e.Message.ObjectName,
                ObjectType = e.Message.ObjectType,
                ObjectStatus = Enum.Parse<ObjectStatus>(@e.Message.ObjectStatus.ToString()),
                MetaData = e.Message.MetaData,
                Changes = e.Message.Changes.Select(c => new ObjectChange(c.FieldName, c.OldValue, c.NewValue)).ToArray()
            })
        .GroupBy(l => l.Source);
        
        var container = _cosmosClient.GetContainer("AuditTrail", "audit-logs");

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