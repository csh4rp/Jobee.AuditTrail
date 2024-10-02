namespace Jobee.AuditTrail.Contracts.EventLogs;

public record EventLogModel
{
    public required string Id { get; init; }
    
    public DateTimeOffset Timestamp { get; init; }
    
    public required string Actor { get; init; }
    
    public required string Subject { get; init; }

    public required string Source { get; init; }
    
    public required Dictionary<string, object> Payload { get; init; }
}