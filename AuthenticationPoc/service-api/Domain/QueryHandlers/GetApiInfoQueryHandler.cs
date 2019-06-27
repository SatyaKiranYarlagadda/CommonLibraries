using System.Threading;
using System.Threading.Tasks;
using service_api.Domain.Models;
using service_api.Domain.Queries;
using MediatR;

namespace service_api.Domain.QueryHandlers
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
