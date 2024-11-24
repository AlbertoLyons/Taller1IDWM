using Microsoft.AspNetCore.Identity;

namespace Taller_1_IDWM.src.Models
{
    // La clase User hereda de IdentityUser<int> para poder utilizar la autenticación de usuarios
    public class User : IdentityUser<int>
    {
        // El rut del usuario
        public string Rut { get; set; } = string.Empty;
        // La fecha de nacimiento del usuario
        public DateOnly Birthdate { get; set; }
        // El género del usuario
        public string Gender { get; set; } = string.Empty;
        // El nombre del usuario
        public string Name { get; set; } = string.Empty;
        // El estado del usuario
        public bool Active { get; set; } = true;
    }
}
