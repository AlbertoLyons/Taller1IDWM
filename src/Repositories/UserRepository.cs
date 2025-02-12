using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.Models;
using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.DTOs.Users;
using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.Mappers;
using Microsoft.AspNetCore.Identity;

namespace Taller_1_IDWM.src.Repositories
{
    public class UserRepository : IUserRepository
    {
        // El contexto de la base de datos
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        /// <summary>
        /// Constructor para el repositorio de usuarios.
        /// </summary>
        /// <param name="dataContext">El contexto de la base de datos.</param>
        public UserRepository(DataContext dataContext, UserManager<User> userManager,  SignInManager<User> signInManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _signInManager = signInManager;

        }
        /// <summary>
        /// Elimina al usuario de la base de datos dado su id.
        /// </summary>
        /// <param name="id"> El id del usuario a eliminar.</param>
        /// <returns>El usuario que fue eliminado, o nulo si no se encontró.</returns>
        /// <exception cref="Exception">Arroja Exception si el usuario no fué encontrado.</exception>
        public async Task<User?> DeleteUserAsync(int id)
        {
            // Buscar el usuario por su id
            var userModel = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            // Si no se encontró el usuario, arrojar una excepción
            if(userModel == null)
            {
                throw new Exception("User not found");
            }
            // Eliminar el usuario de la base de datos
            await _userManager.DeleteAsync(userModel);
            // Guardar los cambios
            await _dataContext.SaveChangesAsync();
            return userModel;
        }

        /// <summary>
        /// Actualiza los parametros del usuario existente.
        /// </summary>
        /// <param name="id">El id del usuario para actualizarse.</param>
        /// <param name="updateUserDTO">El DTO que tiene la información a actualizar..</param>
        /// <returns>True si el cambie fue hecho. False si no.</returns>
        /// <exception cref="Exception">
        /// Arroja error si el usuario no se encontró o si las contraseñas no coinciden.
        /// </exception>
        public async Task<bool?> EditUserAsync(int id, UpdateUserDTO updateUserDTO, string edit)
        {
            if (edit == "password")
            {
            // Buscar el usuario por su id
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new Exception("User not found");
            // Si la contraseña y la confirmación de la contraseña no son nulas y no coinciden, arrojar una excepción
            var result = await _signInManager.CheckPasswordSignInAsync(existingUser, updateUserDTO.ActualPassword, false);
            if (!result.Succeeded) throw new Exception("password");
            if (!existingUser.Active) 
            {
                throw new Exception("User is not active");
            }
            if (updateUserDTO.NewPassword != updateUserDTO.ConfirmPassword)
            {
                throw new Exception("Passwords do not match");
            }
            var changePassword = await _userManager.ChangePasswordAsync(existingUser, updateUserDTO.ActualPassword, updateUserDTO.NewPassword);
            if (!changePassword.Succeeded) throw new Exception("passwordd");
            // Actualizar los campos del usuario

            await _dataContext.SaveChangesAsync();
            return true;
            }

            else{
            
                var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new Exception("User not found");
                if (!existingUser.Active) 
                {
                    throw new Exception("User is not active");
                }
                existingUser.Name = updateUserDTO.Name;
                existingUser.Birthdate = (DateOnly)updateUserDTO.Birthdate;
                existingUser.Gender = updateUserDTO.Gender;
                _dataContext.Users.Update(existingUser);
                await _dataContext.SaveChangesAsync();
                return true;
            }
        }
        /// <summary>
        /// Verifica si existe un usuario con el rut especificado.
        /// </summary>
        /// <param name="rut">El rut a buscar.</param>
        /// <returns>True si el usuario existe, false si no.</returns>
        public async Task<bool> ExistsByRut(string rut)
        {
            return await _dataContext.Users.AnyAsync(p => p.Rut == rut);
        }
        /// <summary>
        /// Checks if a user with the specified ID exists in the database.
        /// </summary>
        /// <param name="id">The ID of the user to check.</param>
        /// <returns>True if the user exists, false otherwise.</returns>
        public async Task<bool> ExistsById(int id)
        {
            if (await _userManager.FindByIdAsync(id.ToString()) == null) return false;
            else {return true;}
        }
        /// <summary>
        /// Obtiene todos los usuarios como una lista de UsuariosDTO, con opciones de filtrado por atributos.
        /// </summary>
        /// <param name="searchTerm"> El termino para buscar con el campo especificado.</param>
        /// <param name="searchField">El campo a buscar. Pueden ser "name", "gender", o "birthdate".</param>
        /// <returns>Una lista que contiene a los UserDTOs segun los filtros que se aplicaron.</returns>
        /// <exception cref="Exception">Arroja si hay algun parametro invalido o si no se cumple alguno de los filtros.</exception>
        public async Task<IEnumerable<UserGetAdminDTO>> GetAllAsync(string searchTerm = "", string searchField = "")
        {
            // Obtener todos los usuarios
            var users = await _userManager.Users.ToListAsync();
            // Si se especificó un campo de búsqueda y un término de búsqueda, filtrar los usuarios
            if (!string.IsNullOrEmpty(searchField) && !string.IsNullOrEmpty(searchTerm))
            {
                // Filtrar los usuarios según el campo de búsqueda
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
                        // En caso de que sea inválido el formato de fecha
                        else
                        {
                            throw new Exception("Invalid birthdate format.");
                        }
                        break;
                    // En caso de que el campo de búsqueda sea inválido
                    default:
                        throw new Exception("Invalid search field.");
                }
            }
            // Convertir los usuarios a UserDTOs
            var UsersDTO = new List<UserGetAdminDTO>();
            foreach (var user in users)
            {
                UsersDTO.Add(user.ToUserGetAdminDTO());
            }
            return UsersDTO;
        }
        /// <summary>
        /// Obtiene a un usario por su id.
        /// </summary>
        /// <param name="id"> El id del usuario a obtener.</param>
        /// <returns>El usuario con la id dada, o nulo si no se encuentra.</returns>
        /// <exception cref="Exception">Arroja si el usuario no se encuentra.</exception>
        public async Task<UserGetAdminDTO?> GetById(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString()) ?? throw new Exception("User not found");
            return user.ToUserGetAdminDTO();
        }
        /// <summary>
        /// Activa o desactiva al usuario con el id especificado.
        /// </summary>
        /// <param name="id">El id del usuario a activar o desactivar.</param>
        /// <returns>El usuario actualizado. Null si el usuario no se encuentra.</returns>
        /// <exception cref="Exception">Arroja si el usuario no se encuentra.</exception>
        public async Task<User?> ActivateDeactivateUserAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if(user == null)
            {
                throw new Exception("User not found");
            }
            user.Active = !user.Active;
            await _userManager.UpdateAsync(user);
            await _dataContext.SaveChangesAsync();
            return user;
        }
        /// <summary>
        /// Obtiene a los usuarios que contengan el <paramref name="username"/> en su nombre.
        /// </summary>
        /// <param name="username">El nombre del usuario a obtener.</param>
        /// <returns>Una lista de <see cref="UserNameDTO"/> que contiene a los usuarios encontrados.</returns>
        /// <exception cref="Exception">Arroja si el <paramref name="username"/> es nulo o vacío.</exception>
        public async Task<IEnumerable<UserNameDTO>> GetByUserName(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new Exception("Username is required");
            }
            var users = await _userManager.Users.Where(p => p.Name.Contains(username)).ToListAsync() ?? throw new Exception("User not found");
            var usersDTO = new List<UserNameDTO>();
            foreach (var user in users)
            {
                usersDTO.Add(user.ToUserNameDTO());
            }
            return usersDTO;
        }
    }
}