using System.Text;
using Jobee.AuditTrail.Contracts.AuditLogs;
using Jobee.AuditTrail.Contracts.Common;
using Jobee.AuditTrail.Domain.AuditLogs;
using Microsoft.Azure.Cosmos;

namespace Jobee.AuditTrail.Functions.Services;

public class AuditLogsService
{
    private readonly CosmosClient _cosmosClient;

    public AuditLogsService(CosmosClient cosmosClient) => _cosmosClient = cosmosClient;

    public async Task<ResultPage<AuditLogModel>> GetAuditLogsAsync(GetAuditLogsQuery query,
        CancellationToken cancellationToken)
    {
        var container = _cosmosClient.GetContainer("audit-trail", "AuditLogs");

        var whereClause = new StringBuilder();
        var parameters = new Dictionary<string, object>();

        if (query.HasAnyFilter)
        {
            whereClause.Append("WHERE 1 = 1");
        }

        if (!string.IsNullOrEmpty(query.Source))
        {
            whereClause.Append(" AND a.PartitionKey = @source");
            parameters.Add("source", query.Source);
        }
        
        parameters.Add("offset", query.PageNumber * query.PageSize);
        parameters.Add("limit", query.PageSize);

        var selectQueryDefinition = new QueryDefinition(
            $"""
            SELECT *
            FROM AuditLogs a
            {whereClause}
            ORDER BY a.Timestamp
            OFFSET @offset LIMIT @limit
            """);
        
        var countQueryDefinition = new QueryDefinition(
            $"""
             SELECT VALUE COUNT (a.id)
             FROM AuditLogs a
             {whereClause}
             """);
        
        foreach (var (key, value) in parameters)
        {
            selectQueryDefinition = selectQueryDefinition.WithParameter(key, value);
            countQueryDefinition = countQueryDefinition.WithParameter(key, value);
        }
        
        using var selectIterator = container.GetItemQueryIterator<AuditLog>(selectQueryDefinition);
        using var countIterator = container.GetItemQueryIterator<int>(countQueryDefinition);

        var result = new List<AuditLogModel>();
        var count = 0;
        
        while (selectIterator.HasMoreResults)
        {
            var feedResponse = await selectIterator.ReadNextAsync(cancellationToken);
            var auditLogs = feedResponse.Select(a => new AuditLogModel
            {
                Id = a.Id,
                Timestamp = a.Timestamp,
                Actor = a.Actor,
                Source = a.Source,
                ObjectName = a.ObjectName,
                ObjectType = a.ObjectType,
                ObjectStatus = Enum.Parse<ObjectStatusModel>(a.ObjectStatus.ToString()),
                MetaData = a.MetaData,
                Changes = a.Changes.Select(c => new ObjectChangeModel(c.FieldName, c.OldValue, c.NewValue)).ToArray(),
            });
            
            result.AddRange(auditLogs);
        }

        while (countIterator.HasMoreResults)
        {
            var feedResponse = await countIterator.ReadNextAsync(cancellationToken);
            count = feedResponse.First();
        }

        return new ResultPage<AuditLogModel>
        {
            Data = result,
            Total = count
        };
    }
}