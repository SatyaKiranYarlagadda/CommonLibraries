using HCF.Common.Foundation.ResponseObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;

namespace Common.Foundation.Api.GlobalFilters
{
    public class ValidateModelStateFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.IsValid)
            {
                return;
            }

            var validationErrors = context.ModelState
                .Keys
                .SelectMany(k => context.ModelState[k].Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();

            var errorResponse = new ApiErrorResponse<string>
            {
                Status = ResponseStatusCode.Fail,
                Message = validationErrors
            };

            context.Result = new ObjectResult(errorResponse);
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
}
