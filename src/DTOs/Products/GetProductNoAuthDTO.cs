using System.ComponentModel.DataAnnotations;

namespace Taller_1_IDWM.src.DTOs.Products
{
    public class GetProductNoAuthDTO
    {
        [Required]
        [StringLength(64, MinimumLength = 10)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"Poleras|Gorros|Jugueteria|Alimentacion|Libros")]
        public string Type { get; set; } = string.Empty;

        [Required]
        [Range(1, 100000000)]
        public int Price { get; set; }

        [Required]
        [Range(1, 100000)]
        public int Stock { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 10)]
        public string ImageUrl { get; set; } = string.Empty;
    }
}