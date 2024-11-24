using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.DTOs.Users;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IUserRepository
    {
        // Método que verifica si un usuario existe por su rut
        Task<bool> ExistsByRut(string rut);
        // Método que verifica si un usuario existe por su id
        Task<bool> ExistsById(int id);
        // Método que obtiene a todos los usuarios dados ciertos parámetros
        Task<IEnumerable<UserDTO>> GetAllAsync(string searchTerm = "", string searchField = "");
        // Método que obtiene a un usuario por su id
        Task<User?> GetById(int id);
        // Método que obtiene a un usuario por su rut
        Task<User?> GetByRut(string rut);
        // Método que actualiza a un usuario dada su id y los datos a actualizar
        Task<IEnumerable<UserNameDTO>> GetByUserName(string username);
        Task<bool?> EditUserAsync(int id, UpdateUserDTO updateUserDTO);
        // Método que elimina a un usuario
        Task<User?> DeleteUserAsync(int id);
        // Método que activa o desactiva a un usuario
        Task<User?> ActivateDeactivateUserAsync(int id);
    }
}