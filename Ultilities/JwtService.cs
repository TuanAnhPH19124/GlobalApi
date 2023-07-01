using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GlobalApi.Ultilities
{
    public static class JwtService
    {
        public static string GenerateJwtToken(IdentityUser user, IEnumerable<string> roles, IConfiguration configuration)
        {
            var jwtTokenHandle = new JwtSecurityTokenHandler();

            var secretkey = configuration.GetSection("JwtConfig:Secret").Value;

            if (string.IsNullOrEmpty(secretkey))
            {
                throw new Exception("Mat Secret key");
            }

            var key = Encoding.UTF8.GetBytes(secretkey);

            var tokendescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString()),

                }),

                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            foreach (var role in roles)
            {
                tokendescriptor.Subject.AddClaim(new Claim(ClaimTypes.Role, role));
            }


            var token = jwtTokenHandle.CreateToken(tokendescriptor);
            var jwtToken = jwtTokenHandle.WriteToken(token);

            return jwtToken;
        }
    }
}
