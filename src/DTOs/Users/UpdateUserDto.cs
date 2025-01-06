using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs.Users
{
    public class UpdateUserDTO
    {
        public DateOnly? Birthdate { get; set; }

        public string? Name { get; set; } = string.Empty;

        public string? Gender { get; set; } = string.Empty;
        
        public string? ActualPassword { get; set; } = string.Empty;
        public string? NewPassword { get; set; } = string.Empty;
        public string? ConfirmPassword { get; set; } = string.Empty;
    }
}