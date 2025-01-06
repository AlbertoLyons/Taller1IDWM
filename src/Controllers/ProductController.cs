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
        /// <summary>
        /// Crea un producto
        /// </summary>
        /// <param name="createProductDTO">Producto a crear</param>
        /// <returns>Url del producto</returns>
        /// <response code="201">Producto creado exitosamente</response>
        /// <response code="400">Error al crear el producto</response>
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
        /// <summary>
        /// Actualiza un producto.
        /// </summary>
        /// <param name="id">Id del producto a actualizar</param>
        /// <param name="updateProductDto">Nuevas características del producto</param>
        /// <returns>Mensaje exitoso</returns>
        /// <response code="200">Producto actualizado exitosamente</response>
        /// <response code="400">Error al actualizar el producto</response>
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
        /// <summary>
        /// Obtener todos los productos.
        /// </summary>
        /// <param name="AscOrDesc">Ordenar los productos por el precio</param>
        /// <param name="pageNumber">Número de paginas</param>
        /// <returns>Todos los productos</returns>
        /// <response code="200">Productos obtenidos exitosamente</response>
        /// <response code="400">Error al obtener los productos</response>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll(string AscOrDesc, string type, string? name , int pageNumber = 1)
        {
            try {
            int pageSize = 10;
            var products = await _productRepository.GetByStock(0);
            products = await _productRepository.GetAscOrDescSorted(0, AscOrDesc, type, name);
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
        /// <summary>
        /// Obtener todos los productos para un usuario admin.
        /// </summary>
        /// <param name="pageNumber">Numero de paginas</param>
        /// <returns>Todos los productos</returns>
        /// <response code="200">Productos obtenidos exitosamente</response>
        /// <response code="400">Error al obtener los productos</response>
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
        /// <summary>
        /// Obtiene un producto por su nombre.
        /// </summary>
        /// <param name="name">Nombre del producto</param>
        /// <param name="pageNumber">Numero de paginas</param>
        /// <returns>Producto/s obtenidos</returns>
        /// <response code="200">Producto/s obtenido/s exitosamente</response>
        /// <response code="404">Producto no encontrado</response>
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
        /// <summary>
        /// Obtiene un producto por su tipo.
        /// </summary>
        /// <param name="type">El tipo del producto</param>
        /// <param name="pageNumber">nNmero de paginas</param>
        /// <returns>Producto/s obtenido/s</returns>
        /// <response code="200">Producto/s obtenido/s exitosamente</response>
        /// <response code="404">Producto no encontrado</response>
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
        /// <summary>
        /// Remueve un producto.
        /// </summary>
        /// <param name="id">Id del producto a remover</param>
        /// <returns></returns>
        /// <response code="200">Producto eliminado exitosamente</response>
        /// <response code="404">Producto no encontrado</response>
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