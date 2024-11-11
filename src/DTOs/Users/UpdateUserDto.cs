using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs.Users
{
    public class UpdateUserDTO
    {
        [Required(AllowEmptyStrings = true)]
        public DateOnly Birthdate { get; set; }

        [Required(AllowEmptyStrings = true)]
        [StringLength(255, MinimumLength = 8)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"Masculino|Femenino|Prefiero no decirlo|otro")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 8)]
        [RegularExpression(@"^[a-zA-Z0-9]*$")]
        public string Password { get; set; } = string.Empty;
    }
}