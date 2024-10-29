using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs
{
    public class CreateProductDTO
    {
        [StringLength(64, MinimumLength = 10)]
        public string Name { get; set; } = string.Empty;
        [RegularExpression(@"Poleras|Gorros|Jugueteria|Alimentacion|Libros")]
        public string Type { get; set; } = string.Empty;
        [Range(1, 100000000)]
        public int Price { get; set; }
        [Range(1, 100000)]
        public int Stock { get; set; }
        public IFormFile Image { get; set; } = null!;
    }
}