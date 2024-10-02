namespace Jobee.AuditTrail.Contracts.AuditLogs;

public record ObjectChangeModel(string FieldName, string? OldValue, string? NewValue);