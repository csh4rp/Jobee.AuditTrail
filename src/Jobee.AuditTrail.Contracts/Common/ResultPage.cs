namespace Jobee.AuditTrail.Contracts.Common;

public record ResultPage<T>
{
    public required int Total { get; init; }

    public required IReadOnlyCollection<T> Data { get; init; }
}