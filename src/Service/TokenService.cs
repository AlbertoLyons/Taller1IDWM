using System.Text;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Models;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;

namespace Taller_1_IDWM.src.Service
{
    public class TokenService : ITokenService
    {
        // Configuraciones
        private readonly IConfiguration _config;
        // Administrador de usuarios de Identity
        private readonly UserManager<User> _userManager;
        // Clave de JWT
        private readonly SymmetricSecurityKey _key;
        /// <summary>
        /// Constructor que recibe la configuración y el administrador de usuarios de Identity.
        /// </summary>
        /// <param name="config">Configuración de la aplicación.</param>
        /// <param name="userManager">Administrador de usuarios de Identity.</param>
        public TokenService(IConfiguration config, UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
            var signinKey = _config["JWT_SIGNINKEY"];
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signinKey!));
        }
        
        /// <summary>
        /// Crea un token JWT para el usuario especificado.
        /// </summary>
        /// <param name="user">Usuario al que se le va a crear el token.</param>
        /// <returns>Token JWT.</returns>
        /// <remarks>
        /// El token contiene los claims NameId (Id en la base de datos), UniqueName, Email y todos los roles del usuario.
        /// </remarks>
        public async Task<string> CreateToken(User user)
        {
            // Crear los claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            };
            // Agregar los roles
            var roles = await _userManager.GetRolesAsync(user);
            // Agregar los roles como claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            // Crear las credenciales
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            // Crear el descriptor del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds,
                Issuer = _config["JWT_IUSSER"],
                Audience = _config["JWT_AUDIENCE"]
            };
            // Crear el token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}