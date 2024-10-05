using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.Models
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        [StringLength(64, MinimumLength = 10)]
        public string Name { get; set; } = string.Empty;
        [RegularExpression(@"Poleras|Gorros|Jugueterpia|Alimentación|Libros")]
        public string Type { get; set; } = string.Empty;
        [Range(1, 100000000)]
        public int Price { get; set; }
        [Range(1, 100000)]
        public int Stock { get; set; }
        [StringLength(256, MinimumLength = 10)]
        public string Image { get; set; } = string.Empty;
    }
}