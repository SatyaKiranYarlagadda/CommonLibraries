using service_api.Domain.Models;
using MediatR;

namespace service_api.Domain.Commands
{
    public class TestPipelineCommand : IRequest<TestPipelineResult>
    {
        public bool? IsValid { get; set; }
    }
}
