using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.DTOs.Products;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IProductRepository
    {
        Task<bool> ExistsById(int id);
        Task<bool> ExistsByNameAndType(string name, string type);
        Task<Product?> AddProductAsync(CreateProductDTO productDto);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetById(int id);
        Task<IEnumerable<GetProductNoAuthDTO>> GetByStock(int stock);
        Task<IEnumerable<Product>> GetByName(string name);
        Task<IEnumerable<Product>> GetByType(string type);
        Task <IEnumerable<GetProductNoAuthDTO>> GetAscOrDescSorted(int stock, string ascOrDesc);
        Task<bool> EditProductAsync(int id, UpdateProductDTO updateProductDTO);
        Task<bool> DeleteProductAsync(Product product);
    }
}