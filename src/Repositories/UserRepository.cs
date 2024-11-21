using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.Models;
using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.DTOs.Users;
using Microsoft.AspNetCore.Identity;

namespace Taller_1_IDWM.src.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        public UserRepository(DataContext dataContext, UserManager<User> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }
        public async Task<User?> DeleteUserAsync(int id)
        {
            var userModel = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if(userModel == null)
            {
                throw new Exception("Product not found");
            }
            _dataContext.Users.Remove(userModel);
            await _dataContext.SaveChangesAsync();
            return userModel;
        }

        public async Task<bool?> EditUserAsync(int id, UpdateUserDTO updateUserDTO)
        {
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new Exception("User not found");
            existingUser.Name = updateUserDTO.Name;
            existingUser.Birthdate = updateUserDTO.Birthdate;
            existingUser.Gender = updateUserDTO.Gender;
            existingUser.PasswordHash = updateUserDTO.Password;
            
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
        
        public Task<User?> GetByRut(string rut)
        {
            return _dataContext.Users.FirstOrDefaultAsync(p => p.Rut == rut);
        }
        public async Task<User?> ActivateDeactivateUserAsync(int id)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(x => x.Id == id);
            if(user == null)
            {
                throw new Exception("User not found");
            }
            user.Active = !user.Active;
            _dataContext.Users.Update(user);
            await _dataContext.SaveChangesAsync();
            return user;
        }
    }
}