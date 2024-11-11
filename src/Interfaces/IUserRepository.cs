using Taller_1_IDWM.src.DTOs.Users;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsByRut(string rut);
        Task<bool> ExistsById(int id);
        Task<bool> AddUserAsync(User user, string password);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetById(int id);
        Task<User?> GetByRut(string rut);
        Task<bool?> EditUserAsync(int id, UpdateUserDTO updateUserDTO);
        Task<User?> DeleteUserAsync(int id);
    }
}