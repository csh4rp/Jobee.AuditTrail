using System.Text.Json.Serialization;

namespace Jobee.AuditTrail.Domain.AuditLogs;

public record AuditLog
{
    public required string Id { get; init; }
    
    public required string PartitionKey { get; init; }
    
    public DateTimeOffset Timestamp { get; init; }
    
    public required string Actor { get; init; }
    
    public required string Source { get; init; } 
    
    public required string ObjectName { get; init; }
    
    public required string ObjectType { get; init; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ObjectStatus ObjectStatus { get; init; }
    
    public required Dictionary<string, object> MetaData { get; init; }
    
    public required ObjectChange[] Changes { get; init; }
}