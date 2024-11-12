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
                if (!ModelState.IsValid) return BadRequest(ModelState);
                else
                {
                    bool exist = await _userManager.FindByEmailAsync(registerDTO.Mail) != null;
                    if(exist) return Conflict("Email already exists");
                    if (registerDTO.Birthdate > DateOnly.Parse(DateTime.Today.ToString())) return BadRequest("Birthdate must be before as today");
                    if (registerDTO.Password != registerDTO.ConfirmPassword) return BadRequest("Passwords do not match");
                    else
                    {
                        var userModel = registerDTO.ToUserFromRegisteredDTO();
                        await _authRepository.RegisterUserAsync(userModel, registerDTO.Password);
                        var newUser = userModel.ToNewUserDTO(await _tokenService.CreateToken(userModel));
                        return Ok(newUser);
                        /*
                        var uri = Url.Action("GetUser", new { id = userModel.Id });
                        var response = new
                        {
                            Message = "User registered successfully",
                            User = userModel.ToUserDTO() 
                        };
                        return Created(uri, response);
                        */
                    } 
                }
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);
                else
                {
                    var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDTO.Email);
                    if(user == null) return Unauthorized("Invalid email or password");

                    var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
                    if (!result.Succeeded) return Unauthorized("Invalid email or password");

                    var loggedUser = user.ToNewUserDTO(await _tokenService.CreateToken(user));
                    return Ok(loggedUser);
                }
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}