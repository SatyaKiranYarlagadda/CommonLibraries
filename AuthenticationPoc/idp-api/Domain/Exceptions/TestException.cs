using HCF.Common.Foundation.ExceptionHandling;

namespace idp_api.Domain.Exceptions
{
    public class TestException : DomainException
    {
        public TestException()
            : base("This is a test exception.")
        {
        }
    }
}
