using Taller_1_IDWM.src.DTOs.Cart;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IReceiptRepository
    {
        // Método que crea un recibo a partir de una lista de productos, el id del usuario y la dirección de envío
        public Task<Receipt> CreateReceipt(List<ProductInCartDTO> products, int userid, AddressDTO address);
        Task<IEnumerable<Receipt>> GetAllAsync();

    }
}