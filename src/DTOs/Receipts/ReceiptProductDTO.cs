using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs
{
    public class ReceiptProductDTO
    {
        [ForeignKey("Receipt")]
        public int ReceiptId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        [Range(1, 100000000)]
        public int UnitPrice { get; set; }
        [Range(1, 100000)]
        public int Quantity { get; set; }
        [Range(1, 1000000000)]
        public int TotalPrice { get; set; }
    }
}