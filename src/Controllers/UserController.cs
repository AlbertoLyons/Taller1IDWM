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
            if(exist){return Conflict("El RUT ya existe");}
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

        [HttpPut]
        [Route("{id}")]

        public async Task<IActionResult> Put([FromRoute] int id , [FromBody] UpdateUserDTO updateUserDto)
        { 
            EmailAddressAttribute emailAttribute = new EmailAddressAttribute();
            List<string> generosValidos = new List<string> { "masculino", "femenino", "otro", "prefiero no decirlo" };
            var userModel = await _userRepository.EditUserAsync(id, updateUserDto);
            if(userModel == null)
            {
                return NotFound();
            }
            var response = new
            {
                Message = "Usuario actualizado exitosamente",
                User = userModel
            };
            return Ok(response);
        }

     [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(int pageNumber = 1)
    {
        int pageSize = 10;
        var users = await _userRepository.GetAllAsync();
        var totalRecords = users.Count();
        var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

        pageNumber = pageNumber < 1 ? 1 : pageNumber > totalPages ? totalPages : pageNumber;

        var paginatedUsers = users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(p => p.ToUserDTO())
            .ToList();

        var response = new
        {
            Message = "Usuarios obtenidos exitosamente",
            TotalRecords = totalRecords,
            TotalPages = totalPages,
            CurrentPage = pageNumber,
            PageSize = pageSize,
            Users = paginatedUsers
        };

        return Ok(response);
    }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try{
                var user = await _userRepository.DeleteUserAsync(id);
            }catch
            {
                return NotFound("Usuario no encontrado");
            }
            return Ok("Usuario eliminado exitosamente");
        }


    }
}