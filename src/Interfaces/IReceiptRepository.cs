using Taller_1_IDWM.src.DTOs.Cart;
using Taller_1_IDWM.src.DTOs.Receipts;
using Taller_1_IDWM.src.DTOs.Users;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IReceiptRepository
    {
        // Método que crea un recibo a partir de una lista de productos, el id del usuario y la dirección de envío
        public Task<Receipt> CreateReceipt(List<ProductInCartDTO> products, int userid, AddressDTO address);
        // Método que retorna todos los recibos
        Task<IEnumerable<Receipt>> GetAllAsync();
        // Método que retorna los recibos por orden histórico
        Task<IEnumerable<ReceiptUserDTO>> GetOrderHistory(int id);
        // Método que retorna los recibos por nombre de usuario
        Task<IEnumerable<Receipt>> GetByUserName(List<UserNameDTO> users);
    }
}