using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.DTOs.Products;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> ExistsById(int id);
        Task<Product?> ExistsByNameAndType(string name, string type);
        Task<Product?> AddProductAsync(CreateProductDTO productDto);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetById(int id);
        Task<IEnumerable<Product>> GetByStock(int stock);
        Task<IEnumerable<Product>> GetByName(string name);
        Task<IEnumerable<Product>> GetByType(string type);
        Task<IEnumerable<Product>> GetAscSorted(int stock);
        Task<IEnumerable<Product>> GetDescSorted(int stock);
        Task<bool> EditProductAsync(int id, UpdateProductDTO updateProductDTO);
        Task<bool> DeleteProductAsync(Product product);
    }
}