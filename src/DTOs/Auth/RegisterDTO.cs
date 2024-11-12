using System.ComponentModel.DataAnnotations;

namespace Taller_1_IDWM.src.DTOs.Auth
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(10, MinimumLength = 8, ErrorMessage = "El RUT debe tener entre 8 y 10 caracteres.")]
        [RegularExpression(@"^\d{7,8}-[0-9Kk]$", ErrorMessage = "El RUT debe estar en el formato 12345678-K, sin puntos y con guion.")]
        public string Rut { get; set; } = string.Empty;
        [Required]
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
        [Required]
        [StringLength(20, MinimumLength = 8)]
        [RegularExpression(@"^[a-zA-Z0-9]*$")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}