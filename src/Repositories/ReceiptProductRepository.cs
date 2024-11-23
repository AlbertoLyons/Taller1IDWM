using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.DTOs.Cart;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Repositories
{
    public class ReceiptProductRepository : IReceiptProductRepository
    {
        private readonly DataContext _dataContext;
        public ReceiptProductRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
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
            List<ReceiptProduct> receiptProducts = new List<ReceiptProduct>();
            foreach (var product in products)
            {
                var receiptProduct = new ReceiptProduct
                {
                    ReceiptId = receiptId,
                    ProductId = product.ID,
                    UnitPrice = product.Price,
                    Quantity = product.Quantity,
                    TotalPrice = product.Price * product.Quantity
                };
                receiptProducts.Add(receiptProduct);
                _dataContext.ReceiptProducts.Add(receiptProduct);
            }
            await _dataContext.SaveChangesAsync();
            return receiptProducts;
        }
    }
}