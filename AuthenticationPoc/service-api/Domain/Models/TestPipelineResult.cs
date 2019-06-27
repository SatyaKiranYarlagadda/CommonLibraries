using HCF.Common.Foundation.CQRSExtensions;

namespace service_api.Domain.Models
{
    public class TestPipelineResult : ICommandResult<string>
    {
        public bool IsSuccess { get; set; }

        public bool IsFailure { get; set; }

        public string Result { get; set; }
    }
}
