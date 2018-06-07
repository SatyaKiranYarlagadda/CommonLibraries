using CorrelationId;
using HCF.Common.Foundation.ExceptionHandling;
using HCF.Common.Foundation.ResponseObjects;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;

namespace Common.Foundation.Api.GlobalFilters
{
    public partial class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment env;
        private readonly ILogger<HttpGlobalExceptionFilter> logger;
        private readonly ICorrelationContextAccessor _correlationContext;

        public HttpGlobalExceptionFilter(IHostingEnvironment env, ILogger<HttpGlobalExceptionFilter> logger, ICorrelationContextAccessor correlationContext)
        {
            this.env = env;
            this.logger = logger;
            _correlationContext = correlationContext;
        }

        public void OnException(ExceptionContext context)
        {
            logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            if (context.Exception is DomainException<object>)
            {
                if (context.Exception is DomainApiResultException<ApiErrorResponse<object>>)
                {
                    var exception = context.Exception as DomainApiResultException<ApiErrorResponse<object>>;
                    context.Result = new ObjectResult(exception.ExceptionDetails);
                    context.HttpContext.Response.StatusCode = (int)exception.HttpStatusCode;
                }
                else if (context.Exception is DomainApiResultException<object>)
                {
                    var exception = context.Exception as DomainApiResultException<object>;
                    context.Result = new ObjectResult(new ApiErrorResponse<object>
                    {
                        Status = ResponseStatusCode.Error,
                        Message = new string[] { exception.Message },
                        Data = exception.ExceptionDetails
                    });
                    context.HttpContext.Response.StatusCode = (int)exception.HttpStatusCode;
                }
                else
                {
                    var exception = context.Exception as DomainException<object>;
                    
                    context.Result = new ObjectResult(new ApiErrorResponse<object>
                    {
                        Status = ResponseStatusCode.Error,
                        Message = new string[] { exception.Message },
                        Data = exception.ExceptionDetails
                    });
                    context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                }
            }
            else
            {
                var errorResponse = new ApiErrorResponse<string>
                {
                    Status = ResponseStatusCode.Error,
                    Message = new string[] { "Unexpected server error. Please try again later. If the problem persists, please contact support.",
                    $"CorrelationId: {_correlationContext.CorrelationContext.CorrelationId}"}
                };

                if (env.IsDevelopment())
                {
                    errorResponse.Data = context.Exception.ToString();
                }

                context.Result = new ObjectResult(errorResponse);
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            context.ExceptionHandled = true;
        }
    }
}
