using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Taller_1_IDWM.src.Models
{
    public class User : IdentityUser<int>
    {
        public string Rut { get; set; } = string.Empty;
        public DateOnly Birthdate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool Active { get; set; } = true;
    }
}
