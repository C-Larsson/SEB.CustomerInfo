using CustomerInfo.REST.Data;
using CustomerInfo.REST.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CustomerInfo.REST.Services.ApiKeyServices
{
    public class ApiKeyService : IApiKeyService
    {

        private readonly IConfiguration _configuration;

        public ApiKeyService(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public bool ValidateKey(string key, out string role)
        {
            role = null;
            var apiKeys = _configuration.GetSection("ApiKeys").Get<Dictionary<string, string>>();
            if (apiKeys.TryGetValue(key, out role))
            {
                return true;
            }
            return false;
        }


        public string GenerateJwtToken(string role)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, role),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenSecret = _configuration.GetSection("AppSettings:Token").Value;

            var key = new SymmetricSecurityKey(Encoding.UTF8.
                GetBytes(tokenSecret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }




    }
}
