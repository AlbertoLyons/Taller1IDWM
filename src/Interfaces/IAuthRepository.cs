using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taller_1_IDWM.src.DTOs.Auth;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IAuthRepository
    {
        Task<string> RegisterUserAsync(RegisterDTO user, string password);
        Task<string> LoginUserAsync(LoginDTO loginDTO);
    }
}