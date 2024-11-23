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
    public class ReceiptRepository : IReceiptRepository
    {
        private readonly DataContext _dataContext;
        public ReceiptRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
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
            return receipt;
        }
    }
}