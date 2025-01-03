using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        // El contexto de la base de datos
        private readonly DataContext _dataContext;
        // Administrador de usuarios de Identity
        private readonly UserManager<User> _userManager;
        // Servicio de tokens
        private readonly ITokenService _tokenService;
        // Administrador de inicio de sesión de Identity
        private readonly SignInManager<User> _signInManager;
        /// <summary>
        /// Constructor del repositorio de autenticación, que se encarga de proveer los métodos para loguear a un usuario y para registrar uno nuevo.
        /// </summary>
        /// /// <param name="dataContext">El contexto de la base de datos.</param>
        /// <param name="userManager">El administrador de usuarios de Identity.</param>
        /// <param name="tokenService">El servicio de tokens.</param>
        /// <param name="signInManager">El administrador de inicio de sesión de Identity.</param>
        public AuthRepository(DataContext dataContext, UserManager<User> userManager, ITokenService tokenService, SignInManager<User> signInManager)
        {
            _dataContext = dataContext;
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        /// <summary>
        /// Inicia sesión de un usuario utilizando sus credenciales.
        /// </summary>
        /// <param name="loginDTO">El DTO que contiene el email y la contraseña del usuario.</param>
        /// <returns>El token JWT generado para el usuario.</returns>
        /// <exception cref="Exception">Arroja si el email o la contraseña son inválidos, o si el usuario no está activo.</exception>
        public async Task<AuthDTO> LoginUserAsync(LoginDTO loginDTO)
        {
            string normalizedEmail = loginDTO.Email.ToUpper();
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == normalizedEmail);
            if(user == null) throw new Exception("Invalid email or password");

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
            if (!result.Succeeded) throw new Exception("Invalid email or password");
            if (!user.Active) 
            {
                throw new Exception("User is not active");
            }
            var newUser = await _tokenService.CreateToken(user);
            var auth = new AuthDTO
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email!,
                Roles = (List<string>)await _userManager.GetRolesAsync(user),
                Token = newUser
            };
            return auth;
        }
        /// <summary>
        /// Registra un usuario en la base de datos.
        /// </summary>
        /// <param name="user">El DTO que contiene la información del usuario a registrar.</param>
        /// <param name="password">La contraseña del usuario.</param>
        /// <returns>El token JWT del usuario recién registrado.</returns>
        /// <exception cref="Exception">Arroja si el email ya existe, o si la fecha de nacimiento es posterior a la fecha actual, o si las contraseñas no coinciden.</exception>
        public async Task<AuthDTO> RegisterUserAsync(RegisterDTO user, string password)
        {
            bool exist = await _userManager.FindByEmailAsync(user.Mail) != null;
            if(exist) throw new Exception("Email already exists");
            if (user.Birthdate > DateOnly.FromDateTime(DateTime.Today)) throw new Exception("Birthdate must be before as today");
            if (user.Password != user.ConfirmPassword) throw new Exception("Passwords do not match");
            // Crea un nuevo usuario
            User newUser = user.ToUserFromRegisteredDTO();
            // Obtiene el resultado de la creación del usuario
            var result = await _userManager.CreateAsync(newUser, password);
            // Si la creación fue exitosa
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "User");
                await _dataContext.SaveChangesAsync();
            }
            // Si la creación falló
            else
            {
                throw new Exception("Error creating user");
            }
            // Crea un token para el usuario
            var registeredUser = await _tokenService.CreateToken(newUser);
            var auth = new AuthDTO
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Email = newUser.Email!,
                Roles = (List<string>)await _userManager.GetRolesAsync(newUser),
                Token = registeredUser
            };
            return auth;
        }
    }
}