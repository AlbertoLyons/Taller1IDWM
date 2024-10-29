using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> ExistsById(int id);
        Task<bool> AddProductAsync(Product product);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetById(int id);
        Task<bool> EditProductAsync(int id, Product product);
        Task<bool> DeleteProductAsync(Product product);
    }
}