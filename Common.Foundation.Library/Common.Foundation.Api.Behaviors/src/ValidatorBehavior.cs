using FluentValidation;
using HCF.Common.Foundation.ExceptionHandling;
using HCF.Common.Foundation.ResponseObjects;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Foundation.Api.Behaviors
{
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IValidator<TRequest>[] _validators;
        public ValidatorBehavior(IValidator<TRequest>[] validators) => _validators = validators;

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any())
            {
                throw new BadRequestApiException<ApiErrorResponse<string>>(
                    $"Command Validation Errors for type {typeof(TRequest).Name}", 
                    new ApiErrorResponse<string>
                    {
                        Status = ResponseStatusCode.Fail,
                        Message = failures.Select(x => x.ErrorMessage).ToArray()
                    });
            }

            var response = await next();
            return response;
        }
    }
}
