using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace ChargesApi.V1.Infrastructure.JWT
{
    public static class Helper
    {
        /// <summary>
        ///   Returns the name of the user used in token
        /// </summary>
        /// <exception cref="ArgumentNullException">Parameter 'token' is null.</exception>
        /// <exception cref="ArgumentException">Value of parameter 'name' in token is null or empty.</exception>
        public static string GetUserName(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token));
            }

            token = token.Replace("Bearer", string.Empty).Trim();

            var claimTypeName = "name";
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);

            var name = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == claimTypeName)?.Value;
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Token doesn't contain a value for 'name'");
            }
            return name;
        }
    }
}
