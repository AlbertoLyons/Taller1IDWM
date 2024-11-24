using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs.Cart
{
    public class ProductInCartDTO
    {
        [Key]
        [Required]
        public int ID { get; set; }
        
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
        [StringLength(256, MinimumLength = 10)]
        public string ImageUrl { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }
}