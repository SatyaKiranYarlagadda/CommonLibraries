using FluentValidation;
using service_api.Domain.Queries;

namespace service_api.Domain.Validators
{
    public class GetApiInfoQueryValidator : AbstractValidator<GetApiInfoQuery>
    {
        public GetApiInfoQueryValidator()
        {
            RuleFor(query => query.IsValid).NotNull().Must(x => x == true);
        }
    }
}
