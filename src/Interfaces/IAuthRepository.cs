using Taller_1_IDWM.src.DTOs.Auth;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IAuthRepository
    {
        // Método que registra a un usuario
        Task<AuthDTO> RegisterUserAsync(RegisterDTO user, string password);
        // Método que loguea a un usuario
        Task<AuthDTO> LoginUserAsync(LoginDTO loginDTO);
    }
}