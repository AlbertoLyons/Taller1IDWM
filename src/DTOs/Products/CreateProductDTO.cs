using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs
{
    public class CreateProductDTO
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

 
        public IFormFile Image { get; set; } = null!;
    }
}