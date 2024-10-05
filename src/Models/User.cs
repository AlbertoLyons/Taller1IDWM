using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Rut { get; set; } = string.Empty;
        public DateOnly Birthdate { get; set; }
        public string Mail { get; set; } = string.Empty;
        [StringLength(255, MinimumLength = 8)]
        public string Name { get; set; } = string.Empty;
         [RegularExpression(@"MASCULINO|FEMENINO|PREFIERO NO DECIRLO|OTRO")]
        public string Gender { get; set; } = string.Empty;
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}