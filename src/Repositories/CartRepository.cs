using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.DTOs.Cart;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly DataContext _dataContext;
        public CartRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<bool> BuyProductAsync(List<ProductInCartDTO> products, int userid, AddressDTO address)
        {
            if (products.Count == 0)
            {
                throw new Exception("Cart is empty");
            }
            if (address == null)
            {
                throw new Exception("Address is required");
            }
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
            _dataContext.Receipts.Add(receipt);
            await _dataContext.SaveChangesAsync();
            foreach (var product in products)
            {
                var receiptProduct = new ReceiptProduct
                {
                    ReceiptId = receipt.Id,
                    ProductId = product.ID,
                    UnitPrice = product.Price,
                    Quantity = product.Quantity,
                    TotalPrice = product.Price * product.Quantity
                };
                _dataContext.ReceiptProducts.Add(receiptProduct);
            }
            await _dataContext.SaveChangesAsync();
            return true;
        }
    }

}