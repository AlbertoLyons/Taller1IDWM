using Microsoft.AspNetCore.Mvc;
using Taller_1_IDWM.src.DTOs.Auth;
using Taller_1_IDWM.src.Interfaces;

namespace Taller_1_IDWM.src.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private static readonly List<string> RevokedTokens = new List<string>();
        public AuthController(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        /// <summary>
        ///     Registra un nuevo usuario en la base de datos.
        /// </summary>
        /// <param name="registerDTO">Recibe el DTO del usuario a registrar</param>
        /// <returns>El token JWT del usuario registrado</returns>
        /// <response code="200">Retorna el token JWT del usuario registrado</response>
        /// <response code="400">Retorna un mensaje de error si el usuario no pudo ser registrado</response>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                var auth = await _authRepository.RegisterUserAsync(registerDTO, registerDTO.Password);
                return Ok(auth);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
        /// <summary>
        ///    Inicia sesión de un usuario en la aplicación.
        /// </summary>
        /// <param name="loginDTO">Este contiene el correo y contraseña ingresada</param>
        /// <returns>El token JWT del usuario logueado</returns>
        /// <response code="200">Retorna el token JWT del usuario logueado</response>
        /// <response code="400">Retorna un mensaje de error si el usuario no pudo ser logueado</response>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var auth = await _authRepository.LoginUserAsync(loginDTO);
                return Ok(auth);
            }
            catch (Exception e)
            {
                if (e.Message == "Invalid email or password")
                {
                    return Unauthorized(new { message = e.Message });
                }
                return BadRequest(new { message = e.Message });
            }
        }
    }
}