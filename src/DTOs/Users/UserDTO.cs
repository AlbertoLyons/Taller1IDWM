using System.ComponentModel.DataAnnotations;
namespace Taller_1_IDWM.src.DTOs
{
    public class UserDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
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
    }
}