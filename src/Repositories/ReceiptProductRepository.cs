using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.DTOs.Cart;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Repositories
{
    public class ReceiptProductRepository : IReceiptProductRepository
    {
        // El contexto de la base de datos
        private readonly DataContext _dataContext;
        /// <summary>
        /// Constructor del repositorio de ReceiptProduct.
        /// </summary>
        /// <param name="dataContext">El contexto de la base de datos.</param>
        public ReceiptProductRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        /// <summary>
        /// Agrega un listado de productos a una factura.
        /// </summary>
        /// <param name="products">El listado de productos a agregar.</param>
        /// <param name="receiptId">El id de la factura.</param>
        /// <returns>El listado de productos agregados a la factura.</returns>
        /// <exception cref="Exception">Si el listado de productos es vac o</exception>
        /// <exception cref="Exception">Si el id de la factura es 0</exception>
        public async Task<IEnumerable<ReceiptProduct>> AddReceiptProduct(List<ProductInCartDTO> products, int receiptId)
        {
            if (products.Count == 0)
            {
                throw new Exception("Cart is empty");
            }
            if (receiptId == 0)
            {
                throw new Exception("Receipt is required");
            }
            // Se crea una lista de productos de la factura
            List<ReceiptProduct> receiptProducts = new List<ReceiptProduct>();
            // Se recorre el listado de productos
            foreach (var product in products)
            {
                // Se crea un nuevo producto de la factura
                var receiptProduct = new ReceiptProduct
                {
                    ReceiptId = receiptId,
                    ProductId = product.ID,
                    UnitPrice = product.Price,
                    Quantity = product.Quantity,
                    TotalPrice = product.Price * product.Quantity
                };
                // Se agrega el producto a la factura
                receiptProducts.Add(receiptProduct);
                // Se agrega el producto a la base de datos
                _dataContext.ReceiptProducts.Add(receiptProduct);
            }
            // Se guardan los cambios en la base de datos
            await _dataContext.SaveChangesAsync();
            return receiptProducts;
        }

        public async Task<IEnumerable<ReceiptProduct>> GetAllAsync()
        {
            var receiptProducts = await _dataContext.ReceiptProducts.ToListAsync();
            return receiptProducts;  
        }
    }
}