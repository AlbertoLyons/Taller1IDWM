using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Admin")]
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
        [HttpPut]
        [Route("{id}")]

        public async Task<IActionResult> Put([FromRoute] int id , [FromBody] UpdateUserDTO updateUserDto)
        { 
            EmailAddressAttribute emailAttribute = new EmailAddressAttribute();
            List<string> generosValidos = new List<string> { "masculino", "femenino", "otro", "prefiero no decirlo" };
            if (updateUserDto.Password != updateUserDto.ConfirmPassword && updateUserDto.Password != null && updateUserDto.ConfirmPassword != null)
            {
                return BadRequest("Passwords do not match");
            }
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
        [HttpPut("{id}/ActivateDeactivate")]
        public async Task<IActionResult> ActivateDeactivateUser(int id)
        {
            var user = await _userRepository.ActivateDeactivateUserAsync(id);
            if(user == null)
            {
                return NotFound("Usuario no encontrado");
            }
            var response = new
            {
                Message = "Usuario actualizado exitosamente",
                active = user.Active
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