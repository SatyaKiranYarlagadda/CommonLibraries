using Microsoft.AspNetCore.Builder;

namespace Common.Foundation.Api.Logging
{
    public static class LoggingContextEnricherBuilderExtension
    {
        public static IApplicationBuilder UseLoggingContextMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingContextEnricherMiddleware>();
        }
    }
}
