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
    //[Authorize(Roles = "Admin")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _productRepository;

        public ProductController(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        [HttpPost("")]
        public async Task<IActionResult> Post([FromForm]CreateProductDTO createProductDTO)
        {
            //try {
                var newProduct = await _productRepository.AddProductAsync(createProductDTO);

                var uri = Url.Action("GetProduct", new { id = newProduct.ID });
                var response = new
                {
                    Message = "Producto creado exitosamente",
                    Product = newProduct.ToProductDTO() 
                };
                return Created(uri, response);
            //} catch (Exception e)
            //{
            //    return BadRequest(new {message = e.message});
            //}
        }
        [HttpPut]
        [Route("{id}")]

        public async Task<IActionResult> Put([FromRoute] int id , [FromBody] UpdateProductDTO updateProductDto)
        { 
            var product = _productRepository.ExistsByNameAndType(updateProductDto.Name, updateProductDto.Type);
            if(product != null){return Conflict("Ya existe un producto con el mismo nombre y tipo");}
            var productModel = await _productRepository.EditProductAsync(id, updateProductDto);
            if(!productModel){return NotFound();}
            var response = new
            {
                Message = "Producto actualizado exitosamente",
                Product = productModel
            };
            return Ok(response);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string AscOrDesc, int pageNumber = 1)
        {
            int pageSize = 10;
            var products = await _productRepository.GetByStock(0);
            if (AscOrDesc == "asc") 
            {
                products = await _productRepository.GetAscSorted(0);
            }
            else if (AscOrDesc == "desc")
            {
                products = await _productRepository.GetDescSorted(0);
            }
            else
            {
                return BadRequest("You should type 'asc' or 'desc'");
            }
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
                Message = "Productos obtenidos exitosamente",
                TotalRecords = totalRecords,
                TotalPages = totalPages,
                CurrentPage = pageNumber,
                PageSize = pageSize,
                Products = paginatedProducts
            };

            return Ok(response);
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
        public async Task<IActionResult> Delete(int id)
        {
            try{
                var product = await _productRepository.GetById(id);
                if(product == null){return BadRequest("El producto con el id ingresado no existe");}
                bool productDelete = await _productRepository.DeleteProductAsync(product);
            }catch
            {
                return NotFound("Producto no encontrado");
            }
            return Ok("Producto eliminado exitosamente");
        }
    }
}