using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.DTOs.Products;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Mappers
{
    public static class ProductMapper
    {
        public static ProductDTO ToProductDTO(this Product productModel)
        {
            return new ProductDTO
            {
                ID = productModel.ID,
                Type = productModel.Type,
                Name = productModel.Name,
                Price = productModel.Price,
                Stock = productModel.Stock,
                ImageUrl = productModel.ImageUrl
            };
        }

        public static Product ToProductFromCreatedDTO(this CreateProductDTO createProductDto)
        {
            return new Product
            {
                Type = createProductDto.Type,
                Name = createProductDto.Name,
                Price = createProductDto.Price,
                Stock = createProductDto.Stock
            };
        }
        public static Product ToProductFromUpdateDTO(this UpdateProductDTO updateProductDTO)
        {
            return new Product
            {
                Type = updateProductDTO.Type,
                Name = updateProductDTO.Name,
                Price = updateProductDTO.Price,
                Stock = updateProductDTO.Stock
            };
        }
        public static GetProductNoAuthDTO ToProductFromNoAuthDTO(this Product product)
        {
            return new GetProductNoAuthDTO
            {
                Type = product.Type,
                Name = product.Name,
                Price = product.Price,
                Stock = product.Stock,
                ImageUrl = product.ImageUrl
            };
        }
    }
}