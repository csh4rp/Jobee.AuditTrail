using Jobee.AuditTrail.Contracts.AuditLogs;
using Jobee.AuditTrail.Functions.Extensions;
using Jobee.AuditTrail.Functions.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace Jobee.AuditTrail.Functions.Functions;

public class AuditLogHttpFunction
{
    private readonly AuditLogsService _auditLogsService;

    public AuditLogHttpFunction(AuditLogsService auditLogsService) => _auditLogsService = auditLogsService;

    [Function("AuditLogHttpFunction")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        var query = req.Query.BindModel<GetAuditLogsQuery>();

        var result = await _auditLogsService.GetAuditLogsAsync(query, req.HttpContext.RequestAborted);
        
        return new OkObjectResult(result);
    }
}