using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs.Users
{
    public class UpdateUserDTO
    {
        public DateTime Birthdate { get; set; }
        [StringLength(255, MinimumLength = 8)]
        public string Name { get; set; } = string.Empty;
         [RegularExpression(@"Masculino|Femenino|Prefiero no decirlo|otro")]
        public string Gender { get; set; } = string.Empty;
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}