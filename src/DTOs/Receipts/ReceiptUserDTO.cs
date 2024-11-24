using System.ComponentModel.DataAnnotations;

namespace Taller_1_IDWM.src.DTOs.Receipts
{
    public class ReceiptUserDTO
    {
        [Required]
        public DateTime BoughtAt { get; set; }
        [Required]
        public int TotalPrice { get; set; }
        // Lista de productos en el recibo
        [Required]
        public List<ReceiptProductUserDTO> ReceiptProducts { get; set; } = new List<ReceiptProductUserDTO>();
    }
}