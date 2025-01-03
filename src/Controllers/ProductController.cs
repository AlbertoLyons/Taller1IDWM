using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Taller_1_IDWM.src.DTOs;
using Taller_1_IDWM.src.DTOs.Products;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Mappers;
using Taller_1_IDWM.src.Models;

namespace Taller_1_IDWM.src.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        [HttpPost("")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromForm]CreateProductDTO createProductDTO)
        {
            try {
                var newProduct = await _productRepository.AddProductAsync(createProductDTO);

                var uri = Url.Action("GetProduct", new { id = newProduct!.ID });
                var response = new
                {
                    Message = "Product created successfully",
                    Product = newProduct.ToProductDTO() 
                };
                return Created(uri, response);
            } catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }
        }
        [HttpPut]
        [Route("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put([FromRoute] int id , [FromForm] UpdateProductDTO updateProductDto)
        { 
            try{
            var productModel = await _productRepository.EditProductAsync(id, updateProductDto);
            var response = new
            {
                Message = "Product updated successfully",
                Product = productModel
            };
            return Ok(response);
            }catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string AscOrDesc, string type, int pageNumber = 1)
        {
            try {
            int pageSize = 10;
            var products = await _productRepository.GetByStock(0);
            products = await _productRepository.GetAscOrDescSorted(0, AscOrDesc, type);
            var totalRecords = products.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            pageNumber = pageNumber < 1 ? 1 : pageNumber > totalPages ? totalPages : pageNumber;

            var paginatedProducts = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                Message = "Products obtained succefully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Products = paginatedProducts
            };

            return Ok(response);
            }catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }
        }
        [HttpGet("GetAllAdmin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllAdmin(int pageNumber = 1)
        {
            try {
            int pageSize = 10;
            var products = await _productRepository.GetByStock(-1);
            var totalRecords = products.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            pageNumber = pageNumber < 1 ? 1 : pageNumber > totalPages ? totalPages : pageNumber;

            var paginatedProducts = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var response = new
            {
                Message = "Products obtained succefully",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Products = paginatedProducts
            };

            return Ok(response);
            }catch (Exception e)
            {
                return BadRequest(new {message = e.Message});
            }
        }
        [HttpGet("Name")]
        public async Task<IActionResult> GetByName(string name, int pageNumber = 1)
        {
            int pageSize = 10;
            var products = await _productRepository.GetByName(name);
            if(products == null){return NotFound("Producto no encontrado");}
            var totalRecords = products.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            pageNumber = pageNumber < 1 ? 1 : pageNumber > totalPages ? totalPages : pageNumber;

            var paginatedProducts = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => p.ToProductDTO())
                .ToList();

            var response = new
            {
                Message = "Producto/s obtenido/s exitosamente",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Products = paginatedProducts
            };
            return Ok(response);
        }
        [HttpGet("Type")]
        public async Task<IActionResult> GetByType(string type, int pageNumber = 1)
        {
            int pageSize = 10;
            var products = await _productRepository.GetByType(type);
            if(products == null){return NotFound("Producto no encontrado");}
            var totalRecords = products.Count();
            var totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            pageNumber = pageNumber < 1 ? 1 : pageNumber > totalPages ? totalPages : pageNumber;

             var paginatedProducts = products
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => p.ToProductFromNoAuthDTO())
                .ToList();

            var response = new
            {
                Message = "Producto/s obtenido/s exitosamente",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Products = paginatedProducts
            };
            return Ok(response);
        }
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try{
                var product = await _productRepository.GetById(id);
                bool productDelete = await _productRepository.DeleteProductAsync(product!);
            }catch
            {
                return NotFound("Producto no encontrado");
            }
            return Ok("Producto eliminado exitosamente");
        }
    }
}