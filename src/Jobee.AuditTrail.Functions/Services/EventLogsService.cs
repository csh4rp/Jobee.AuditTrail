using System.Text;
using Jobee.AuditTrail.Contracts.AuditLogs;
using Jobee.AuditTrail.Contracts.Common;
using Jobee.AuditTrail.Contracts.EventLogs;
using Jobee.AuditTrail.Domain.EventLogs;
using Microsoft.Azure.Cosmos;

namespace Jobee.AuditTrail.Functions.Services;

public class EventLogsService
{
    private readonly CosmosClient _cosmosClient;

    public EventLogsService(CosmosClient cosmosClient) => _cosmosClient = cosmosClient;

    public async Task<ResultPage<EventLogModel>> GetEventLogsAsync(GetEventLogsQuery query,
        CancellationToken cancellationToken)
    {
        var container = _cosmosClient.GetContainer("audit-trail", "EventLogs");

        var whereClause = new StringBuilder();
        var parameters = new Dictionary<string, object>();

        if (query.HasAnyFilter)
        {
            whereClause.Append("WHERE 1 = 1");
        }

        if (!string.IsNullOrEmpty(query.Source))
        {
            whereClause.Append(" AND e.PartitionKey = @source");
            parameters.Add("source", query.Source);
        }
        
        parameters.Add("offset", query.PageNumber * query.PageSize);
        parameters.Add("limit", query.PageSize);

        var selectQueryDefinition = new QueryDefinition(
            $"""
            SELECT *
            FROM EventLogs e
            {whereClause}
            ORDER BY e.Timestamp
            OFFSET @offset LIMIT @limit
            """);
        
        var countQueryDefinition = new QueryDefinition(
            $"""
             SELECT VALUE COUNT (e.id)
             FROM EventLogs e
             {whereClause}
             """);
        
        foreach (var (key, value) in parameters)
        {
            selectQueryDefinition = selectQueryDefinition.WithParameter(key, value);
            countQueryDefinition = countQueryDefinition.WithParameter(key, value);
        }
        
        using var selectIterator = container.GetItemQueryIterator<EventLog>(selectQueryDefinition);
        using var countIterator = container.GetItemQueryIterator<int>(countQueryDefinition);

        var result = new List<EventLogModel>();
        var count = 0;
        
        while (selectIterator.HasMoreResults)
        {
            var feedResponse = await selectIterator.ReadNextAsync(cancellationToken);
            var eventLogs = feedResponse.Select(e => new EventLogModel
            {
                Id = e.Id,
                Timestamp = e.Timestamp,
                Actor = e.Actor,
                Source = e.Source,
                Subject = e.Subject,
                Payload = e.Payload
            });
            
            result.AddRange(eventLogs);
        }

        while (countIterator.HasMoreResults)
        {
            var feedResponse = await countIterator.ReadNextAsync(cancellationToken);
            count = feedResponse.First();
        }

        return new ResultPage<EventLogModel>
        {
            Data = result,
            Total = count
        };
    }
}