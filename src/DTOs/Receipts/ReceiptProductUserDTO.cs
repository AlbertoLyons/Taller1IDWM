using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Taller_1_IDWM.src.DTOs.Receipts
{
    public class ReceiptProductUserDTO
    {
        public string Name { get; set; } = String.Empty;
        public string Type { get; set; } = String.Empty;
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int TotalPrice { get; set; }
    }
}