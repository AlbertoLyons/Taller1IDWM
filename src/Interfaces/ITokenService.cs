using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface ITokenService
    {
        // Método que crea un token para un usuario
        Task<string> CreateToken(User user);
    }
}