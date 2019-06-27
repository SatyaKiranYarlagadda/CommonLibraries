using FluentValidation;
using service_api.Domain.Commands;

namespace service_api.Domain.Validators
{
    public class TestPipelineCommandValidator : AbstractValidator<TestPipelineCommand>
    {
        public TestPipelineCommandValidator()
        {
            RuleFor(query => query.IsValid).NotNull().Must(x => x == true);
        }
    }
}
