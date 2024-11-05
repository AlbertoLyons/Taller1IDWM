using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Taller_1_IDWM.src.DTOs.Users;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Mappers;

namespace Taller_1_IDWM.src.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]CreateUserDTO createUserDTO)
        {
            bool exist = await _userRepository.ExistsByRut(createUserDTO.Rut);
            EmailAddressAttribute emailAttribute = new EmailAddressAttribute();
            List<string> generosValidos = new List<string> { "masculino", "femenino", "otro", "prefiero no decirlo" };
            if(exist)
            {
                return Conflict("El RUT ya existe");
            }
            else
            {
                var userModel = createUserDTO.ToUserFromCreatedDTO();
                await _userRepository.AddUserAsync(userModel);
                var uri = Url.Action("GetUser", new { id = userModel.Id });

                var response = new
                {
                    Message = "Usuario creado exitosamente",
                    User = userModel.ToUserDTO() 
                };
                return Created(uri, response);
            }
        }

    }
}