using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Taller_1_IDWM.src.DTOs.Users;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Mappers;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserController(IUserRepository userRepository, UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [HttpPost("Dev")]
        public async Task<IActionResult> AssignRoleToUser(int id, string role)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            var roleExists = await _roleManager.RoleExistsAsync(role);

            if (!roleExists)
            {
                return BadRequest($"El rol '{role}' no existe.");
            }

            if (user != null)
            {
                var result = await _userManager.AddToRoleAsync(user, role);
                if (result.Succeeded)
                {
                    return Ok("Rol asignado exitosamente.");
                }
                else
                {
                    return BadRequest($"No se pudo asignar el rol. Errores: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            return BadRequest("Usuario no encontrado.");
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
                await _userRepository.AddUserAsync(userModel, createUserDTO.Password);
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