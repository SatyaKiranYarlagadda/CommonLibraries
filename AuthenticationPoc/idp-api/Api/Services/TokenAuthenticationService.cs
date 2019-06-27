using idp_api.Api.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace idp_api.Api.Services
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly TokenManagement _tokenManagement;
        private readonly UserAccess _userAccess;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public TokenAuthenticationService(IHttpContextAccessor httpContextAccessor, IOptions<TokenManagement> tokenManagement, IOptions<UserAccess> userAccess)
        {
            _httpContextAccessor = httpContextAccessor;
            _tokenManagement = tokenManagement.Value;
            _userAccess = userAccess.Value;
        }
        public bool IsAuthenticated(out string token)
        {
            var user = _httpContextAccessor.HttpContext.User.Identity.Name;
            token = string.Empty;

            var userAccessDetail = _userAccess.UserAccessDetail?.FirstOrDefault(x => string.Equals(x.User, user, StringComparison.InvariantCultureIgnoreCase));

            var claims = new List<Claim>
            {
                new Claim("User", user)                
            };

            if (userAccessDetail != null)
            {
                claims.AddRange(
                    new[]
                    {
                        new Claim("IsMedical", userAccessDetail.IsMedical.ToString()),
                        new Claim("IsAncillary", userAccessDetail.IsAncillary.ToString()),
                        new Claim("IsHospital", userAccessDetail.IsHospital.ToString())
                    });
            }
            else
            {
                return false;
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                _tokenManagement.Issuer,
                _tokenManagement.Audience,
                claims,
                expires: DateTime.Now.AddSeconds(_tokenManagement.AccessExpiration),
                signingCredentials: credentials
            );
            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            return true;

        }
    }
}
