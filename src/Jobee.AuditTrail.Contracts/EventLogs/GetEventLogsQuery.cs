namespace Jobee.AuditTrail.Contracts.EventLogs;

public record GetEventLogsQuery
{
    public string? Source { get; set; }
    
    public int PageSize { get; set; } = 20;
    
    public int PageNumber { get; set; }

    public bool HasAnyFilter => !string.IsNullOrEmpty(Source);
}