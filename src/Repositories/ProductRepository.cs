using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.DTOs.Products;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Models;
using Taller_1_IDWM.src.Mappers;

namespace Taller_1_IDWM.src.Repositories
{
    public class ProductRepository : IProductRepository
    {
        // El contexto de la base de datos
        private readonly DataContext _dataContext;
        // El servicio de Cloudinary
        private readonly Cloudinary _cloudinary;
        
        /// <summary>
        /// Constructor del repositorio de productos.
        /// </summary>
        /// <param name="dataContext">El contexto de la base de datos.</param>
        /// <param name="cloudinary">El servicio de Cloudinary.</param>
        public ProductRepository(DataContext dataContext, Cloudinary cloudinary)
        {
            _dataContext = dataContext;
            _cloudinary = cloudinary;
        }
        
        /// <summary>
        /// Agrega un producto a la base de datos.
        /// </summary>
        /// <param name="productDto">El objeto con los datos del producto a agregar.</param>
        /// <returns>El objeto del producto agregado.</returns>
        /// <exception cref="Exception">Si ya existe un producto con el mismo nombre y tipo.</exception>
        /// <exception cref="Exception">Si la imagen no es un PNG o JPG.</exception>
        public async Task<Product?> AddProductAsync(CreateProductDTO productDto)
        {
            var existingproduct = await ExistsByNameAndType(productDto.Name, productDto.Type);
            if(existingproduct){throw new Exception("Already exists a product with this name and type");}
            if (productDto.Image == null)
            {
                throw new Exception("Image is required");
            }
            if (productDto.Image.ContentType != "image/png" && productDto.Image.ContentType != "image/jpg")
            {
                throw new Exception("Image must be a PNG or JPG");
            }
            // Realiza los párametros de subida de la imagen
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(productDto.Image.FileName, productDto.Image.OpenReadStream()),
                Folder = "products_image"
            };
            // Sube la imagen a Cloudinary
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            // Si hay un error en la subida de la imagen, lanza una excepción
            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }
            // Crea el producto
            var product = new Product
            {
                Name = productDto.Name,
                Type = productDto.Type,
                Price = productDto.Price,
                Stock = productDto.Stock,
                // Obtiene la URL de la imagen subida
                ImageUrl = uploadResult.SecureUrl.AbsoluteUri
            };
            // Agrega el producto a la base de datos
            _dataContext.Products.Add(product);
            await _dataContext.SaveChangesAsync();
            return product;
        }
        /// <summary>
        /// Elimina un producto de la base de datos.
        /// </summary>
        /// <param name="product">El objeto del producto a eliminar.</param>
        /// <returns>True si el producto fue eliminado.</returns>
        /// <exception cref="Exception">Si el producto no existe.</exception>
        public async Task<bool> DeleteProductAsync(Product product)
        {
            if (product == null)
            {
                throw new Exception("Product not found");
            }   
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Actualiza un producto de la base de datos.
        /// </summary>
        /// <param name="id">El id del producto a actualizar.</param>
        /// <param name="updateProductDTO">El objeto con la información a actualizar del producto.</param>
        /// <returns>True si el producto fue actualizado.</returns>
        /// <exception cref="Exception">Si el producto no existe o ya existe un producto con el mismo nombre y tipo.</exception>
        public async Task<bool> EditProductAsync(int id, UpdateProductDTO updateProductDTO)
        {
            var product = await ExistsByNameAndType(updateProductDTO.Name, updateProductDTO.Type);
            if(product){throw new Exception("Already exists a product with this name and type");}
            var existingProduct = await _dataContext.Products.FirstOrDefaultAsync(p => p.ID == id);
            if(existingProduct == null){
                throw new Exception("Product not found");
            }
            // Realiza los párametros de subida de la imagen
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(updateProductDTO.Image.FileName, updateProductDTO.Image.OpenReadStream()),
                Folder = "products_image"
            };
            // Sube la imagen a Cloudinary
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            // Si hay un error en la subida de la imagen, lanza una excepción
            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }
            // Actualiza el producto
            existingProduct.Name = updateProductDTO.Name;
            existingProduct.Type = updateProductDTO.Type;
            existingProduct.Price = updateProductDTO.Price;
            existingProduct.Stock = updateProductDTO.Stock;
            existingProduct.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            // Actualiza el producto en la base de datos
            _dataContext.Products.Update(existingProduct);
            await _dataContext.SaveChangesAsync();
            return true;
        }
        /// <summary>
        /// Verifica si existe un producto con el id especificado.
        /// </summary>
        /// <param name="id">El id del producto a buscar.</param>
        /// <returns>True si el producto existe, false si no.</returns>
        public async Task<bool> ExistsById(int id)
        {
            return await _dataContext.Products.AnyAsync(p => p.ID == id);
        }
        /// <summary>
        /// Verifica si existe un producto con el nombre y tipo especificados.
        /// </summary>
        /// <param name="name">El nombre del producto a buscar.</param>
        /// <param name="type">El tipo del producto a buscar.</param>
        /// <returns>True si el producto existe, false si no.</returns>
        public async Task<bool> ExistsByNameAndType(string name, string type)
        {
            return await _dataContext.Products.AnyAsync(p => p.Name == name && p.Type == type);
        }
        /// <summary>
        /// Obtiene todos los productos de la base de datos.
        /// </summary>
        /// <returns>Una lista que contiene a todos los productos.</returns>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = await _dataContext.Products.ToListAsync();
            return products;
        }
        /// <summary>
        /// Obtiene todos los productos que tengan un stock mayor o igual al especificado.
        /// </summary>
        /// <param name="stock">El stock minimo que deben tener los productos.</param>
        /// <returns>Una lista que contiene a todos los productos que cumplan con el stock minimo.</returns>
        public async Task<IEnumerable<GetProductNoAuthDTO>> GetByStock(int stock)
        {
            var products = await _dataContext.Products.Where(p => p.Stock >= stock).ToListAsync();
            var productsDTO = new List<GetProductNoAuthDTO>();
            foreach (var product in products)
            {
                productsDTO.Add(product.ToProductFromNoAuthDTO());
            }
            return productsDTO;
        }
        /// <summary>
        /// Obtiene todos los productos que contengan el nombre especificado.
        /// </summary>
        /// <param name="name">El nombre que se busca en los productos.</param>
        /// <returns>Una lista que contiene a todos los productos que contengan el nombre especificado.</returns>
        /// <exception cref="Exception">Si no se encontraron productos.</exception>
        public async Task<IEnumerable<Product>> GetByName(string name)
        {
            return await _dataContext.Products.Where(p => p.Name.Contains(name)).ToListAsync() ?? throw new Exception("Product/s not found");
        }
        /// <summary>
        /// Obtiene todos los productos que tengan el tipo especificado.
        /// </summary>
        /// <param name="type">El tipo de los productos a obtener.</param>
        /// <returns>Una lista que contiene a todos los productos que tengan el tipo especificado.</returns>
        /// <exception cref="Exception">Si no se encontraron productos.</exception>
        public async Task<IEnumerable<Product>> GetByType(string type)
        {
            return await _dataContext.Products.Where(p => p.Type == type).ToListAsync() ?? throw new Exception("Product/s not found");
        }
        /// <summary>
        /// Obtiene todos los productos que tengan un stock mayor al especificado, ordenados por precio en orden ascendente o descendente.
        /// </summary>
        /// <param name="stock">El stock minimo que deben tener los productos.</param>
        /// <param name="ascOrDesc">El orden en que se van a ordenar los productos. Puede ser "asc" o "desc".</param>
        /// <returns>Una lista que contiene a todos los productos que cumplan con el stock minimo, ordenados por precio.</returns>
        /// <exception cref="Exception">Si el stock es menor o igual a 0, o si el orden especificado no es "asc" ni "desc".</exception>
        public async Task<IEnumerable<GetProductNoAuthDTO>> GetAscOrDescSorted(int stock, string ascOrDesc, string type)
        {
            // Obtiene la lista de productos en un DTO
            var ProductsDTO = new List<GetProductNoAuthDTO>();
            // Si se tiene que ordenar de manera ascendente

            if(ascOrDesc == "asc")
            {
                var products = await _dataContext.Products.OrderBy(p => p.Price).Where(p => p.Stock > stock).ToListAsync();
                foreach (var product in products)
                {
                    ProductsDTO.Add(product.ToProductFromNoAuthDTO());
                }
                if (type != "" && type != "Nada")
                {
                    ProductsDTO = ProductsDTO.Where(p => p.Type == type).ToList();
                }
                return ProductsDTO;
            }
            // Si se tiene que ordenar de manera descendente
            else if (ascOrDesc == "desc")
            {
                var products = await _dataContext.Products.OrderByDescending(p => p.Price).Where(p => p.Stock > stock).ToListAsync();
                foreach (var product in products)
                {
                    ProductsDTO.Add(product.ToProductFromNoAuthDTO());
                }
                if (type != "" && type != "Nada")
                {
                    ProductsDTO = ProductsDTO.Where(p => p.Type == type).ToList();
                }
                return ProductsDTO;
            }
            // Si el orden no es "asc" ni "desc"
            else
            {
                throw new Exception("You should type 'asc' or 'desc'");
            }
        }
        /// <summary>
        /// Obtiene un producto por su ID.
        /// </summary>
        /// <param name="id">El id del producto a obtener.</param>
        /// <returns>El producto con el ID especificado, o null si no existe.</returns>
        public Task<Product?> GetById(int id)
        {
            return _dataContext.Products.FirstOrDefaultAsync(p => p.ID == id);
        }
    }
}