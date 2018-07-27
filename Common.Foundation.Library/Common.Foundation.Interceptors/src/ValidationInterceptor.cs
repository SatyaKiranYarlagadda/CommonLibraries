using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using FluentValidation;
using FluentValidation.Results;
using HCF.Common.Foundation.ExceptionHandling;
using HCF.Common.Foundation.ResponseObjects;

namespace Common.Foundation.Interceptors.src
{
    public class ValidationInterceptor : IInterceptor
    {
        private readonly IValidator[] _validators;

        public ValidationInterceptor(IValidator[] validators)
        {
            _validators = validators;
        }

        public void Intercept(IInvocation invocation)
        {
            var validationErrors = new List<ValidationFailure>();
            foreach (var request in invocation.Arguments)
            {
                var failures = _validators
                    .Where(v => v.CanValidateInstancesOfType(request.GetType()))
                    .Select(v => v.Validate(request))
                    .SelectMany(result => result.Errors)
                    .Where(error => error != null)
                    .ToList();

                validationErrors.AddRange(failures);
            }


            if (validationErrors.Any())
            {
                throw new BadRequestApiException(
                    $"Validation Errors for method {invocation.Method.Name}",
                    new ApiErrorResponse<object>
                    {
                        Status = ResponseStatusCode.Fail,
                        Message = validationErrors.Select(x => x.ErrorMessage).ToArray()
                    });
            }

            invocation.Proceed();
        }
    }
}
