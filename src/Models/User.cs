using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.Models

{
    public class User
    {
        public int Id { get; set; }
        public string Rut { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
        public string Mail { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}