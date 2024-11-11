using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Taller_1_IDWM.src.Data;
using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.DTOs.Products;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly DataContext _dataContext;
        private readonly Cloudinary cloudinary;
        public ProductRepository(DataContext dataContext) 
        {
            _dataContext = dataContext;
        }
        
        public async Task<Product?> AddProductAsync(CreateProductDTO productDto)
        {
            
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(productDto.Image.FileName, productDto.Image.OpenReadStream()),
                Folder = "products_image"
            };

            var uploadResult = await cloudinary.UploadAsync(uploadParams);
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
            var existingProduct = await _dataContext.Products.FirstOrDefaultAsync(p => p.ID == id);
            if(existingProduct == null){
                return false;
            }
            existingProduct.Name = updateProductDTO.Name;
            existingProduct.Type = updateProductDTO.Type;
            existingProduct.Price = updateProductDTO.Price;
            existingProduct.Stock = updateProductDTO.Stock;


           // existingProduct.ImageUrl = updateProductDTO.ImageUrl;
            
            _dataContext.Products.Update(existingProduct);
            await _dataContext.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsById(int id)
        {
            return await _dataContext.Products.AnyAsync(p => p.ID == id);
        }

        public async Task<Product?> ExistsByNameAndType(string name, string type)
        {
            var products = await _dataContext.Products.FirstOrDefaultAsync(p => p.Name == name && p.Type == type );
            return products;
            
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _dataContext.Products.ToListAsync();
        }

        public Task<Product?> GetById(int id)
        {
            return _dataContext.Products.FirstOrDefaultAsync(p => p.ID == id);
        }
    }
}