namespace Jobee.AuditTrail.Contracts.AuditLogs;

public record GetAuditLogsQuery
{
    public string? Source { get; set; }
    
    public int PageSize { get; set; } = 20;
    
    public int PageNumber { get; set; }

    public bool HasAnyFilter => !string.IsNullOrEmpty(Source);
}