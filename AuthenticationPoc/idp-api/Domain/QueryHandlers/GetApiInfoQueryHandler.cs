using System.Threading;
using System.Threading.Tasks;
using idp_api.Domain.Models;
using idp_api.Domain.Queries;
using MediatR;

namespace idp_api.Domain.QueryHandlers
{
    public class GetApiInfoQueryHandler : IRequestHandler<GetApiInfoQuery, ApiInfo>
    {
        public async Task<ApiInfo> Handle(GetApiInfoQuery request, CancellationToken cancellationToken)
        {
            return await Task.FromResult(new ApiInfo
            {
                ApiName = "[TBD-ApiName]",
                ApiVersion = "[TBD-ApiVersion]"
            });
        }
    }
}
