using service_api.Domain.Models;
using MediatR;

namespace service_api.Domain.Queries
{
    public class GetApiInfoQuery : IRequest<ApiInfo>
    {
        public bool? IsValid { get; set; }
    }
}
