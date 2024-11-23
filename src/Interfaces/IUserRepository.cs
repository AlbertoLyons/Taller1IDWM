using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.DTOs.Users;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsByRut(string rut);
        Task<bool> ExistsById(int id);
        Task<IEnumerable<UserDTO>> GetAllAsync(string searchTerm = "", string searchField = "");
        Task<User?> GetById(int id);
        Task<User?> GetByRut(string rut);
        Task<bool?> EditUserAsync(int id, UpdateUserDTO updateUserDTO);
        Task<User?> DeleteUserAsync(int id);
        Task<User?> ActivateDeactivateUserAsync(int id);
    }
}