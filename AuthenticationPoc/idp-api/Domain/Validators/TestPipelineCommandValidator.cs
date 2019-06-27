using FluentValidation;
using idp_api.Domain.Commands;

namespace idp_api.Domain.Validators
{
    public class TestPipelineCommandValidator : AbstractValidator<TestPipelineCommand>
    {
        public TestPipelineCommandValidator()
        {
            RuleFor(query => query.IsValid).NotNull().Must(x => x == true);
        }
    }
}
