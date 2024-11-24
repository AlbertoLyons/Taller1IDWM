using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taller_1_IDWM.src.DTOs.Cart;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Mappers
{
    public static class CartMapper
    {
        public static ProductInCartDTO ToProductInCartDTO(this Product product)
        {
            return new ProductInCartDTO
            {
                ID = product.ID,
                Type = product.Type,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                Quantity = 1
            };
        }
    }
}