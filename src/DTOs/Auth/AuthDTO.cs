using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs.Auth
{
    public class AuthDTO
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public List<string> Roles { get; set; } = new List<string>();
        public string Token { get; set; } = "";
    }
}