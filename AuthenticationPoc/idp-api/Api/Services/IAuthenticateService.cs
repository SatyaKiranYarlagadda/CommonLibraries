namespace idp_api.Api.Services
{
    public interface IAuthenticateService
    {
        bool IsAuthenticated(out string token);
    }
}
