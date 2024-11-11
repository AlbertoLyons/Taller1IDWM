using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        public AuthController(UserManager<User> userManager, IAuthRepository authRepository)
        {
            _userManager = userManager;
            _authRepository = authRepository;
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
                    else
                    {
                        var userModel = registerDTO.ToUserFromRegisteredDTO();
                        await _authRepository.RegisterUserAsync(userModel, registerDTO.Password);
                        var uri = Url.Action("GetUser", new { id = userModel.Id });
                        var response = new
                        {
                            Message = "User registered successfully",
                            User = userModel.ToUserDTO() 
                        };
                        return Created(uri, response);
                    } 
                }
            }
            catch
            {
                return StatusCode(500, "Internal server error ");
            }
        }
    }
}