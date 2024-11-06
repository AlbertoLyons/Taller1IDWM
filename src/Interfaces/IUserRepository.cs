using Taller_1_IDWM.src.DTOs.Users;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> ExistsByRut(string rut);
        Task<bool> ExistsById(int id);
        Task<bool> AddUserAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetById(int id);
        Task<bool?> EditUserAsync(int id, UpdateUserDTO updateUserDTO);
        Task<bool> DeleteUserAsync(User user);
    }
}