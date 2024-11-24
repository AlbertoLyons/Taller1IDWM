using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.DTOs.Cart;
using Taller_1_IDWM.src.DTOs.Users;
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

        /// <summary>
        /// Obtiene todos los recibos de la base de datos.
        /// </summary>
        /// <returns>Una lista que contiene a todos los recibos.</returns>
        public async Task<IEnumerable<Receipt>> GetAllAsync()
        {
            var receipt = await _dataContext.Receipts.ToListAsync();
            return receipt;  
        }
        /// <summary>
        /// Obtiene todos los recibos de un usuario en especifico.
        /// </summary>
        /// <param name="id">El id del usuario.</param>
        /// <returns>Una lista que contiene a todos los recibos del usuario.</returns>
        public async Task<IEnumerable<Receipt>> GetOrderHistory(int id)
        {
            var receipt = await _dataContext.Receipts.Where(x => x.UserId == id).ToListAsync();
            return receipt;  
        }
        /// <summary>
        /// Obtiene todos los recibos de una lista de usuarios.
        /// </summary>
        /// <param name="users">La lista de usuarios.</param>
        /// <returns>Una lista que contiene a todos los recibos de los usuarios.</returns>
        /// <exception cref="Exception">Si no se encontraron recibos.</exception>
        public async Task<IEnumerable<Receipt>> GetByUserName(List<UserNameDTO> users)
        {
            var receipts = new List<Receipt>();
            // Itera primero sobre los usuarios
            foreach (var user in users)
            {
                // Obtiene los recibos de cada usuario
                var receiptsDB = await _dataContext.Receipts.Where(r => r.UserId == user.id).ToListAsync();
                // Agrega los recibos a la lista
                receipts.AddRange(receiptsDB);
            }
            if (receipts.Count == 0)
            {
                throw new Exception("No receipts found");
            }
            return receipts;  
        }
    }
}