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
    [Authorize]
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
            try {
                var userModel = await _userRepository.EditUserAsync(id, updateUserDto);
                var response = new
                {
                    Message = "User updated successfully",
                    User = userModel
                };
                return Ok(response);
            } catch (Exception e) {
                return BadRequest(new {error = e.Message});
            }
        }
        [HttpPut("{id}/ActivateDeactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateDeactivateUser(int id)
        {
            try 
            {
                var user = await _userRepository.ActivateDeactivateUserAsync(id);
                var response = new
                {
                    Message = "Usuario actualizado exitosamente",
                    active = user.Active
                };
                return Ok(response);
            } catch (Exception e){
                return BadRequest(new {error = e.Message});
            }

        }
        [HttpGet("GetAll")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, string searchTerm = "", string searchField = "")
        {
            try {
            int pageSize = 10;
            var users = await _userRepository.GetAllAsync(searchTerm, searchField);
            var totalRecords = users.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            pageNumber = pageNumber < 1 ? 1 : pageNumber > totalPages ? totalPages : pageNumber;

            var paginatedUsers = users
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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
            } catch (Exception e) {
                return BadRequest(new {error = e.Message});
            }
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try{
                var user = await _userRepository.DeleteUserAsync(id);
            }catch (Exception e)
            {
                return NotFound(new {error = e.Message});
            }
            return Ok("User deleted successfully");
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            try{
                var user = await _userRepository.GetById(id);
            }catch (Exception e)
            {
                return NotFound(new {error = e.Message});
            }
            return Ok("User deleted successfully");
        }
    }
}