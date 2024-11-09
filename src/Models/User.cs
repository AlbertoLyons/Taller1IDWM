using Microsoft.AspNetCore.Identity;

namespace Taller_1_IDWM.src.Models
{
    public class User : IdentityUser<int>
    {
        public string Rut { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
