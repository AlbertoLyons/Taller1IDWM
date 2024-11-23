using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taller_1_IDWM.src.DTOs.Cart;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface ICartRepository
    {
        public Task<bool> BuyProductAsync(List<ProductInCartDTO> products, int userid, AddressDTO address);
    }
}