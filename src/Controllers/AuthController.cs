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