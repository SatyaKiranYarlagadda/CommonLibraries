using CorrelationId;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Common.Foundation.Api.Logging
{
    public class LoggingContextEnricherMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ICorrelationContextAccessor _correlationContextAccessor;

        public LoggingContextEnricherMiddleware(RequestDelegate next, ICorrelationContextAccessor correlationContextAccessor)
        {
            _next = next;
            _correlationContextAccessor = correlationContextAccessor;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            using (LogContext.PushProperty("CorrelationId", _correlationContextAccessor.CorrelationContext.CorrelationId))
            using (LogContext.PushProperty("Hostname", Environment.MachineName))
            using (LogContext.PushProperty("HostIP", Dns.GetHostAddresses(Environment.MachineName)[0].ToString()))
            using (LogContext.PushProperty("RemoteIP", httpContext.Connection.RemoteIpAddress))
            using (LogContext.PushProperty("RequestURI", $"{httpContext.Request.Scheme}://{httpContext.Request.Host}{httpContext.Request.Path}{httpContext.Request.QueryString}"))
            using (LogContext.PushProperty("RequestMethod", httpContext.Request.Method))
            using (LogContext.PushProperty("User", httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value))
            {
                await _next.Invoke(httpContext);
            }
        }
    }
}
