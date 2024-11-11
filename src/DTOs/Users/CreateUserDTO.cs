using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs.Users
{
    public class CreateUserDTO
    {
        [Required]
        public string Rut { get; set; } = string.Empty;

        [Required]
        //[RegularExpression(@"^\d{4}-(0[1-9]|1[0-2])-(0[1-9]|[12][0-9]|3[01])$", ErrorMessage = "La fecha debe estar en el formato yyyy-MM-dd.")]
        public DateOnly Birthdate { get; set; }

        [Required]
        public string Mail { get; set; } = string.Empty;

        [Required]
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