using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.DTOs.Products;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Interfaces
{
    public interface IProductRepository
    {
        // Método que verifica si existe un producto dado su id
        Task<bool> ExistsById(int id);
        // Método que verifica si existe un producto dado su nombre y tipo
        Task<bool> ExistsByNameAndType(string name, string type);
        // Método que añade a un producto
        Task<Product?> AddProductAsync(CreateProductDTO productDto);
        // Método que obtiene todos los productos
        Task<IEnumerable<Product>> GetAllAsync();
        // Método que obtiene un producto por su id
        Task<Product?> GetById(int id);
        // Método que obtiene un producto que sea mayor a un stock dado
        Task<IEnumerable<GetProductNoAuthDTO>> GetByStock(int stock);
        // Método que obtiene un producto que contiene un nombre dado
        Task<IEnumerable<Product>> GetByName(string name);
        // Método que obtiene un producto que tenga un tipo dado
        Task<IEnumerable<Product>> GetByType(string type);
        // Método que obtiene productos en orden ascendente o descendente dado un stock
        Task <IEnumerable<GetProductNoAuthDTO>> GetAscOrDescSorted(int stock, string ascOrDesc, string type);
        // Método que actualiza un producto dada su id y los datos a actualizar
        Task<bool> EditProductAsync(int id, UpdateProductDTO updateProductDTO);
        // Método que elimina un producto dado el objeto
        Task<bool> DeleteProductAsync(Product product);
    }
}