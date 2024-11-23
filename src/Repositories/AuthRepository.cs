using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.DTOs.Auth;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Mappers;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dataContext;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<User> _signInManager;

        public AuthRepository(DataContext dataContext, UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }

        public async Task<string> LoginUserAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == loginDTO.Email);
            if(user == null) throw new Exception("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if (!result.Succeeded) throw new Exception("Invalid email or password");
            if (!user.Active) 
            {
                throw new Exception("User is not active");
            }
            var newUser = await _tokenService.CreateToken(user);
            return newUser;
        }


        public async Task<string> RegisterUserAsync(RegisterDTO user, string password)
        {
            bool exist = await _userManager.FindByEmailAsync(user.Mail) != null;
            if(exist) throw new Exception("Email already exists");
            if (user.Birthdate > DateOnly.FromDateTime(DateTime.Today)) throw new Exception("Birthdate must be before as today");
            if (user.Password != user.ConfirmPassword) throw new Exception("Passwords do not match");
            User newUser = user.ToUserFromRegisteredDTO();

            var result = await _userManager.CreateAsync(newUser, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "User");
                await _dataContext.SaveChangesAsync();
            }
            var registeredUser = await _tokenService.CreateToken(newUser);
            return registeredUser;
        }
    }
}