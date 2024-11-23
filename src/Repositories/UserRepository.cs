using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.Models;
using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.DTOs.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.Mappers;

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
                throw new Exception("User not found");
            }
            _dataContext.Users.Remove(userModel);
            await _dataContext.SaveChangesAsync();
            return userModel;
        }

        public async Task<bool?> EditUserAsync(int id, UpdateUserDTO updateUserDTO)
        {
            var existingUser = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id) ?? throw new Exception("User not found");
            if (updateUserDTO.Password == null && updateUserDTO.ConfirmPassword == null)
            {
                updateUserDTO.Password = existingUser.PasswordHash!;
                updateUserDTO.ConfirmPassword = existingUser.PasswordHash!;
            }
            if (updateUserDTO.Password != updateUserDTO.ConfirmPassword && updateUserDTO.Password != null && updateUserDTO.ConfirmPassword != null)
            {
                throw new Exception("Passwords do not match");
            }

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

        public async Task<IEnumerable<UserDTO>> GetAllAsync(string searchTerm = "", string searchField = "")
        {
            var users = await _dataContext.Users.ToListAsync();
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
                            throw new Exception("Invalid birthdate format.");
                        }
                        break;
                    default:
                        throw new Exception("Invalid search field.");
                }
            }
            var UsersDTO = new List<UserDTO>();
            foreach (var user in users)
            {
                UsersDTO.Add(user.ToUserDTO());
            }
            return UsersDTO;
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