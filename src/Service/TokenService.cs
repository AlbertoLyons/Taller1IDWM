using System.Text;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Taller_1_IDWM.src.Service
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _config = config;
            var signinKey = _config["JWT_SIGNINKEY"];
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signinKey));
        }
        public string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName!)
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT_IUSSER"],
                Audience = _config["JWT_AUDIENCE"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    internal class SymetricSecurityKey
    {
    }
}