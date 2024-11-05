using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.Models;
using Microsoft.EntityFrameworkCore;

namespace Taller_1_IDWM.src.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        public UserRepository(DataContext dataContext)  
        {
            _dataContext = dataContext;
        }
        public async Task<bool> AddUserAsync(User user)
        {
            _dataContext.Users.Add(user);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(User user)
        {
            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool?> EditUserAsync(int id, User user)
        {
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new Exception("User not found");
            existingUser.Name = user.Name;
            existingUser.Birthdate = user.Birthdate;
            existingUser.Gender = user.Gender;
            
            _dataContext.Users.Update(existingUser);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByRut(string rut)
        {
            return await _dataContext.Users.AnyAsync(p => p.Rut == rut);
        }
        public async Task<bool> ExistsById(int id)
        {
            return await _dataContext.Users.AnyAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _dataContext.Users.ToListAsync();
        }
        public async Task<User?> GetById(int id)
        {
            return await _dataContext.Users.FirstOrDefaultAsync(p => p.Id == id) ?? throw new Exception("User not found");
        }
    
    }
}