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
        private readonly DataContext _dataContext;
        private readonly Cloudinary _cloudinary;
        public ProductRepository(DataContext dataContext, Cloudinary cloudinary)
        {
            _dataContext = dataContext;
            _cloudinary = cloudinary;
        }
        
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
            
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(productDto.Image.FileName, productDto.Image.OpenReadStream()),
                Folder = "products_image"
            };
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }

            var product = new Product
            {
                Name = productDto.Name,
                Type = productDto.Type,
                Price = productDto.Price,
                Stock = productDto.Stock,
                ImageUrl = uploadResult.SecureUrl.AbsoluteUri
            };
            _dataContext.Products.Add(product);

            await _dataContext.SaveChangesAsync();

            return product;
        }

        public async Task<bool> DeleteProductAsync(Product product)
        {
            _dataContext.Products.Remove(product);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EditProductAsync(int id, UpdateProductDTO updateProductDTO)
        {
            var product = await ExistsByNameAndType(updateProductDTO.Name, updateProductDTO.Type);
            if(product){throw new Exception("Already exists a product with this name and type");}
            var existingProduct = await _dataContext.Products.FirstOrDefaultAsync(p => p.ID == id);
            if(existingProduct == null){
                throw new Exception("Product not found");
            }

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(updateProductDTO.Image.FileName, updateProductDTO.Image.OpenReadStream()),
                Folder = "products_image"
            };
            //await _cloudinary.delete
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);
            if (uploadResult.Error != null)
            {
                throw new Exception(uploadResult.Error.Message);
            }
            existingProduct.Name = updateProductDTO.Name;
            existingProduct.Type = updateProductDTO.Type;
            existingProduct.Price = updateProductDTO.Price;
            existingProduct.Stock = updateProductDTO.Stock;
            existingProduct.ImageUrl = uploadResult.SecureUrl.AbsoluteUri;
            
            _dataContext.Products.Update(existingProduct);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsById(int id)
        {
            return await _dataContext.Products.AnyAsync(p => p.ID == id);
        }
        public async Task<bool> ExistsByNameAndType(string name, string type)
        {
            return await _dataContext.Products.AnyAsync(p => p.Name == name && p.Type == type);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var products = await _dataContext.Products.ToListAsync();
            return products;
        }
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
        public async Task<IEnumerable<Product>> GetByName(string name)
        {
            return await _dataContext.Products.Where(p => p.Name.Contains(name)).ToListAsync();
        }
        public async Task<IEnumerable<Product>> GetByType(string type)
        {
            return await _dataContext.Products.Where(p => p.Type == type).ToListAsync();
        }
        public async Task<IEnumerable<GetProductNoAuthDTO>> GetAscOrDescSorted(int stock, string ascOrDesc)
        {
            var ProductsDTO = new List<GetProductNoAuthDTO>();
            if(ascOrDesc == "asc")
            {
                var products = await _dataContext.Products.OrderBy(p => p.Price).Where(p => p.Stock > stock).ToListAsync();
                foreach (var product in products)
                {
                    ProductsDTO.Add(product.ToProductFromNoAuthDTO());
                }
                return ProductsDTO;
            }
            else if (ascOrDesc == "desc")
            {
                var products = await _dataContext.Products.OrderByDescending(p => p.Price).Where(p => p.Stock > stock).ToListAsync();
                foreach (var product in products)
                {
                    ProductsDTO.Add(product.ToProductFromNoAuthDTO());
                }
                return ProductsDTO;
            }
            else
            {
                throw new Exception("You should type 'asc' or 'desc'");
            }
        }
        public Task<Product?> GetById(int id)
        {
            return _dataContext.Products.FirstOrDefaultAsync(p => p.ID == id);
        }
    }
}