using FluentValidation;
using idp_api.Domain.Queries;

namespace idp_api.Domain.Validators
{
    public class GetApiInfoQueryValidator : AbstractValidator<GetApiInfoQuery>
    {
        public GetApiInfoQueryValidator()
        {
            RuleFor(query => query.IsValid).NotNull().Must(x => x == true);
        }
    }
}
