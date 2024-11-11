using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        public AuthRepository(DataContext dataContext, UserManager<User> userManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
        }
        public async Task<bool> RegisterUserAsync(User user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _dataContext.SaveChangesAsync();
            }
            return true;
        }
    }
}