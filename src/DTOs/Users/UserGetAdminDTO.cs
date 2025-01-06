using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs.Users
{
    public class UserGetAdminDTO
    {
        public int Id { get; set; }

        public string Rut { get; set; } = string.Empty;

        public DateOnly Birthdate { get; set; }

        public string Mail { get; set; } = string.Empty;
        
        [StringLength(255, MinimumLength = 8)]
        public string Name { get; set; } = string.Empty;

        [RegularExpression(@"Masculino|Femenino|Prefiero no decirlo|otro")]
        public string Gender { get; set; } = string.Empty;
        
    }
}