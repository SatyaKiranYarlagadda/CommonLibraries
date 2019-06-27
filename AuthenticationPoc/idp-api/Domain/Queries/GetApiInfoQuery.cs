using idp_api.Domain.Models;
using MediatR;

namespace idp_api.Domain.Queries
{
    public class GetApiInfoQuery : IRequest<ApiInfo>
    {
        public bool? IsValid { get; set; }
    }
}
