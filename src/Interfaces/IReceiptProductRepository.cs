using Taller_1_IDWM.src.DTOs.Cart;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IReceiptProductRepository
    {
        // Método que crea productos dados su recibo. Obtiene una lista de productos y el id del recibo al que pertenecen.
        public Task<IEnumerable<ReceiptProduct>> AddReceiptProduct(List<ProductInCartDTO> products, int receiptId);
        // Método que obtiene los productos de un recibo dado su id.
        public Task<IEnumerable<ReceiptProduct>> GetByReceiptId(int receiptId);
    }
}