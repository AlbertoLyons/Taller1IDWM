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
    //[Authorize(Roles = "Admin")]
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
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll(int pageNumber = 1, string searchTerm = "", string searchField = "")
        {
            int pageSize = 10;
            var users = await _userRepository.GetAllAsync();

            // Si searchField está vacío, no se aplica ningún filtrado
            if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchTerm))
            {
                switch (searchField.ToLower())
                {
                    case "name":
                        users = users.Where(u => u.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "gender":
                        users = users.Where(u => u.Gender.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
                        break;
                    case "birthdate":
                        DateOnly? dateOnly = null;

                        // Intentamos parsear el searchTerm directamente
                        if (searchTerm.Length == 4 && int.TryParse(searchTerm, out var year))
                        {
                            // Buscar todos los usuarios nacidos en ese año (sin importar el mes ni el día)
                            dateOnly = new DateOnly(year, 1, 1);
                        }
                        else if (searchTerm.Length == 7 && DateTime.TryParse(searchTerm + "-01", out var date))
                        {
                            // Buscar todos los usuarios nacidos en ese mes del año
                            dateOnly = DateOnly.FromDateTime(date);
                        }
                        else if (searchTerm.Length == 10 && DateTime.TryParse(searchTerm, out var fullDate))
                        {
                            // Buscar todos los usuarios nacidos en esa fecha exacta
                            dateOnly = DateOnly.FromDateTime(fullDate);
                        }

                        if (dateOnly != null)
                        {
                            // Filtrar por año, mes o fecha completa según el caso
                            if (searchTerm.Length == 4)  // Solo año
                            {
                                users = users.Where(u => u.Birthdate.Year == dateOnly.Value.Year).ToList();
                            }
                            else if (searchTerm.Length == 7)  // Año y mes
                            {
                                users = users.Where(u => u.Birthdate.Year == dateOnly.Value.Year && u.Birthdate.Month == dateOnly.Value.Month).ToList();
                            }
                            else if (searchTerm.Length == 10)  // Año, mes y día
                            {
                                users = users.Where(u => u.Birthdate == dateOnly.Value).ToList();
                            }
                        }
                        else
                        {
                            return BadRequest("Invalid birthdate format.");
                        }
                        break;
                    default:
                        return BadRequest("Invalid search field.");
                }
            }

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