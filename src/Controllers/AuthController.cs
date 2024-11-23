using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sprache;
using Taller_1_IDWM.src.DTOs.Auth;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Mappers;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;
        private static readonly List<string> RevokedTokens = new List<string>();
        public AuthController(UserManager<User> userManager, IAuthRepository authRepository, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _authRepository = authRepository;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                var token = await _authRepository.RegisterUserAsync(registerDTO, registerDTO.Password);
                return Ok(token);
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
                var token = await _authRepository.LoginUserAsync(loginDTO);
                return Ok(token);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}