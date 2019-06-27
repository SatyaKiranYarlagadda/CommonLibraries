using System.Threading;
using System.Threading.Tasks;
using idp_api.Domain.Commands;
using idp_api.Domain.Exceptions;
using idp_api.Domain.Models;
using HCF.Common.Foundation.CQRSExtensions;
using MediatR;

namespace idp_api.Domain.CommandHandlers
{
    public class TestPipelineCommandHandler : IRequestHandler<TestPipelineCommand, TestPipelineResult>
    {
        public async Task<TestPipelineResult> Handle(TestPipelineCommand request, CancellationToken cancellationToken)
        {
            if (request.IsValid.HasValue && request.IsValid == true)
            {
                return await Task.FromResult(new TestPipelineResult
                {
                    IsSuccess = true,
                    Result = "Healthy"
                });
            }
            else
            {
                throw new TestException();
            }
        }
    }
}
