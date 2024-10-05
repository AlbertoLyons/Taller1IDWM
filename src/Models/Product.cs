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
        int ID { get; set; }
        [StringLength(64, MinimumLength = 10)]
        string Name { get; set; }
        [RegularExpression(@"Poleras|Gorros|Jugueterpia|Alimentación|Libros")]
        string Type { get; set; }
        [Range(1, 100000000)]
        int Price { get; set; }
        [Range(1, 100000)]
        int Stock { get; set; }
        [StringLength(256, MinimumLength = 10)]
        string Image { get; set; }
    }
}