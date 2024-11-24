using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.DTOs.Cart;
using Taller_1_IDWM.src.DTOs.Receipts;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Mappers;
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
        /// <summary>
        /// Obtiene una lista de productos asociados a un recibo específico por su ID.
        /// </summary>
        /// <param name="receiptId">El ID del recibo cuyos productos se desean obtener.</param>
        /// <returns>Una lista de productos asociados al recibo especificado.</returns>
        public async Task<IEnumerable<ReceiptProduct>> GetByReceiptId(int receiptId)
        {
            var receiptProducts = await _dataContext.ReceiptProducts.Where(x => x.ReceiptId == receiptId).ToListAsync();
            return receiptProducts;  
        }
        public async Task<IEnumerable<ReceiptProductUserDTO>> GetByReceiptIdUser(int receiptId)
        {
            List<ReceiptProductUserDTO> receiptProductsUserDTO = [];
            // Se obtienen los productos asociados al recibo
            var receiptProducts = await _dataContext.ReceiptProducts.Where(x => x.ReceiptId == receiptId).ToListAsync();
            // Se recorre el listado de productos
            foreach (var receiptProduct in receiptProducts)
            {
                var productId = receiptProduct.ProductId;
                var name = _dataContext.Products.Where(x => x.ID == productId).Select(x => x.Name).FirstOrDefault();
                var type = _dataContext.Products.Where(x => x.ID == productId).Select(x => x.Type).FirstOrDefault();
                // Se mapea el producto de la factura al DTO
                var receiptProductUserDTO = receiptProduct.ReceiptProductToReceiptProductUserDTO();
                receiptProductUserDTO.Name = name!;
                receiptProductUserDTO.Type = type!;
                receiptProductsUserDTO.Add(receiptProductUserDTO);
            }
            return receiptProductsUserDTO;
        }
    }
}