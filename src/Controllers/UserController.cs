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
        /// <summary>
        /// Actualiza un usuario en la base de datos.
        /// </summary>
        /// <param name="id">Id del usuario</param>
        /// <param name="updateUserDto">Nueva informaciómn del usuario</param>
        /// <returns>Respuesta de la base de datos</returns>
        /// <status code="200">Si el usuario se actualiza exitosamente.</status>
        /// <status code="400">Si ocurre un error al actualizar el usuario.</status>
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateUser([FromRoute] int id , [FromBody] UpdateUserDTO updateUserDto)
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
        /// <summary>
        /// Activa o desactiva un usuario en la base de datos.
        /// </summary>
        /// <param name="id">Usuario a activar o desactivar</param>
        /// <returns>Nuevo estado del usuario</returns>
        /// <status code="200">Si el usuario se activa o desactiva exitosamente.</status>
        /// <status code="400">Si ocurre un error al activar o desactivar el usuario.</status>
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
                    active = user!.Active
                };
                return Ok(response);
            } catch (Exception e){
                return BadRequest(new {error = e.Message});
            }

        }
        /// <summary>
        /// Obtiene todos los usuarios de la base de datos.
        /// </summary>
        /// <param name="pageNumber">Numero de pagina</param>
        /// <param name="searchTerm">Año de la creacion del usuario</param>
        /// <param name="searchField">Término de busqueda('gender', 'name' o 'birthdate')</param>
        /// <returns>JSON de usuarios</returns>
        /// <status code="200">Si los usuarios se obtienen exitosamente.</status>
        /// <status code="400">Si ocurre un error al obtener los usuarios.</status>
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
        /// <summary>
        /// Borra un usuario de la base de datos.
        /// </summary>
        /// <param name="id">Id del usuario</param>
        /// <returns></returns>
        /// <status code="200">Si el usuario se borra exitosamente.</status>
        /// <status code="400">Si ocurre un error al borrar el usuario.</status>
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
        /// <summary>
        /// Obtiene un usuario por su id.
        /// </summary>
        /// <param name="id">Id del usuario a obtener</param>
        /// <returns>El usuario obtenido</returns>
        /// <status code="200">Si el usuario se obtiene exitosamente.</status>
        /// <status code="400">Si no se encuentra el usuario.</status>
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(int id)
        {
            try{
                var user = await _userRepository.GetById(id);
                return Ok(user);
            }catch (Exception e)
            {
                return NotFound(new {error = e.Message});
            }
        }
    }
}