namespace Jobee.AuditTrail.Domain.AuditLogs;

public record ObjectChange(string FieldName, string? OldValue, string? NewValue);