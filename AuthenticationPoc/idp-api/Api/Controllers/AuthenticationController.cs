using idp_api.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace idp_api.Api.Controllers
{
    [Route("api/v1")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;

        public AuthenticationController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        [HttpGet("authenticate")]
        public async Task<IActionResult> Get()
        {
            if (_authenticateService.IsAuthenticated(out string token))
            {
                var result = await Task.FromResult(new { Token = token });
                return Ok(result);
            }

            return Unauthorized();
        }
    }
}
