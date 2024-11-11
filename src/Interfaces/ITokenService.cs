using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}