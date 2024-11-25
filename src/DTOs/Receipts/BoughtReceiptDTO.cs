using System.ComponentModel.DataAnnotations;

namespace Taller_1_IDWM.src.DTOs.Receipts
{
    public class BoughtReceiptDTO
    {
        [Required]
        public int id { get; set; }
        [Required]
        public string Country { get; set; } = string.Empty;
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string County { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty; 
        [Required]
        public DateTime BoughtAt { get; set; }
        [Required]
        public int TotalPrice { get; set; }
        // Lista de productos en el recibo
        [Required]
        public List<ReceiptProductUserDTO> ReceiptProducts { get; set; } = new List<ReceiptProductUserDTO>();
    }
}