using System.ComponentModel.DataAnnotations;
namespace Taller_1_IDWM.src.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Rut { get; set; } = string.Empty;
        public DateTime Birthdate { get; set; }
        public string Mail { get; set; } = string.Empty;
        [StringLength(255, MinimumLength = 8)]
        public string Name { get; set; } = string.Empty;
         [RegularExpression(@"Masculino|Femenino|Prefiero no decirlo|otro")]
        public string Gender { get; set; } = string.Empty;
        [StringLength(20, MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}