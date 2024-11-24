using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs.Auth
{
    public class NewUserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}