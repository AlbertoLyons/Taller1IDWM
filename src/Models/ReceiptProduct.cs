using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Taller_1_IDWM.src.Models
{
    public class ReceiptProduct
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