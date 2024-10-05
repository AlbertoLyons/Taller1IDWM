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
        public int Birthdate { get; set; }
        public int Mail { get; set; }
        public int MyProperty { get; set; }
        public int Gender { get; set; }
        public int Password { get; set; }
    }
}