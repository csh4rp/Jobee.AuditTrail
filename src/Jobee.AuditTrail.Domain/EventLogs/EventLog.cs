namespace Jobee.AuditTrail.Domain.EventLogs;

public class EventLog
{
    public required string Id { get; init; }
    
    public required string PartitionKey { get; init; }
    
    public DateTimeOffset Timestamp { get; init; }
    
    public required string Actor { get; init; }
    
    public required string Subject { get; init; }

    public required string Source { get; init; }
    
    public required Dictionary<string, object> Payload { get; init; }
}