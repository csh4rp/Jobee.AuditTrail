using Jobee.AuditTrail.Contracts.AuditLogs;
using Jobee.AuditTrail.Contracts.EventLogs;
using Jobee.AuditTrail.Functions.Extensions;
using Jobee.AuditTrail.Functions.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jobee.AuditTrail.Functions.Functions;

public class EventLogHttpFunction
{
    private readonly ILogger<EventLogHttpFunction> _logger;
    private readonly EventLogsService _eventLogsService;

    public EventLogHttpFunction(ILogger<EventLogHttpFunction> logger, EventLogsService eventLogsService)
    {
        _logger = logger;
        _eventLogsService = eventLogsService;
    }

    [Function("EventLogHttpFunction")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        var query = req.Query.BindModel<GetEventLogsQuery>();

        var result = await _eventLogsService.GetEventLogsAsync(query, req.HttpContext.RequestAborted);
        
        return new OkObjectResult(result);
    }
}