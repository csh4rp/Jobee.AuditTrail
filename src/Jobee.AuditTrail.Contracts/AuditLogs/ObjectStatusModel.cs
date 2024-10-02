using System.Runtime.Serialization;

namespace Jobee.AuditTrail.Contracts.AuditLogs;

public enum ObjectStatusModel
{
    [EnumMember(Value = "CREATED")]
    Created = 1,
    
    [EnumMember(Value = "MODIFIED")]
    Modified = 2,
    
    [EnumMember(Value = "DELETED")]
    Deleted = 3
}