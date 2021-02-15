using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace CustomJwtApi
{
    public class BearerRequirement : IAuthorizationRequirement
    {
        public async Task<bool> IsTokenValid(SomeValidationContext context, string token)
        {
            await Task.Run(() => { });

            var handler = new JwtSecurityTokenHandler();

            var jsonToken = handler.ReadToken(token);

            var jwtSecurityTokens = handler.ReadToken(token) as JwtSecurityToken;

            // here you can check if the token received is valid 
            return true;
        }
    }
}