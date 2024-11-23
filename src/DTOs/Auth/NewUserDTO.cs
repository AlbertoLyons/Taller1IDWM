using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs.Auth
{
    public class NewUserDTO
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
    }
}