using System.Text.Json.Serialization;

namespace Jobee.AuditTrail.Contracts.AuditLogs;

public record AuditLogModel
{
    public required string Id { get; init; }
    
    public DateTimeOffset Timestamp { get; init; }
    
    public required string Actor { get; init; }
    
    public required string Source { get; init; } 
    
    public required string ObjectName { get; init; }
    
    public required string ObjectType { get; init; }
    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required ObjectStatusModel ObjectStatus { get; init; }
    
    public required Dictionary<string, object> MetaData { get; init; }
    
    public required ObjectChangeModel[] Changes { get; init; }
}