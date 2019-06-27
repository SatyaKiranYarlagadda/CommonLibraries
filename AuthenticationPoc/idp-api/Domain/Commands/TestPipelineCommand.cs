using idp_api.Domain.Models;
using MediatR;

namespace idp_api.Domain.Commands
{
    public class TestPipelineCommand : IRequest<TestPipelineResult>
    {
        public bool? IsValid { get; set; }
    }
}
