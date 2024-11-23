using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.DTOs.Cart;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Repositories
{
    public class ReceiptRepository : IReceiptRepository
    {
        // El contexto de la base de datos
        private readonly DataContext _dataContext;
        /// <summary>
        /// Constructor para ReceiptRepository.
        /// </summary>
        /// /// <param name="dataContext">El DataContext a usar</param>
        public ReceiptRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Crea un nuevo recibo dado los productos y la dirección.
        /// </summary>
        /// <param name="products">Los productos en el carrito.</param>
        /// <param name="userid">El id del usuario que realizó la compra.</param>
        /// <param name="address">La dirección del usuario.</param>
        /// <returns>El recibo creado.</returns>
        /// <exception cref="Exception">Se arroja si el carrito está vacío o si la dirección esta nula.</exception>
        public async Task<Receipt> CreateReceipt(List<ProductInCartDTO> products, int userid, AddressDTO address)
        {
            if (products.Count == 0)
            {
                throw new Exception("Cart is empty");
            }
            if (address == null)
            {
                throw new Exception("Address is required");
            }
            // Crear el recibo
            var receipt = new Receipt
            {
                UserId = userid,
                BoughtAt = DateTime.Now,
                TotalPrice = products.Sum(p => p.Price * p.Quantity),
                Country = address.Country,
                City = address.City,
                County = address.County,
                Address = address.Address
            };
            // Crear los productos del recibo
            _dataContext.Receipts.Add(receipt);
            // Guardar los cambios
            await _dataContext.SaveChangesAsync();
            return receipt;
        }
    }
}