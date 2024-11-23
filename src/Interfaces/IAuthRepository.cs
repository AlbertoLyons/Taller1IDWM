using Taller_1_IDWM.src.DTOs.Auth;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IAuthRepository
    {
        // Método que registra a un usuario
        Task<string> RegisterUserAsync(RegisterDTO user, string password);
        // Método que loguea a un usuario
        Task<string> LoginUserAsync(LoginDTO loginDTO);
    }
}