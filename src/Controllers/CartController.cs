using System.Text.Json;
using Taller_1_IDWM.src.Models;
using Microsoft.AspNetCore.Mvc;
using Taller_1_IDWM.src.Interfaces;
using Taller_1_IDWM.src.Mappers;
using Taller_1_IDWM.src.DTOs.Cart;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace Taller_1_IDWM.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IReceiptRepository _receiptRepository;
        private readonly IReceiptProductRepository _receiptProductRepository;
        private const string ProductCookieKey = "ProductsList";
        private const string UserCookieKey = "UserGUID";
        public CartController(IProductRepository productRepository, IReceiptRepository receiptRepository, IReceiptProductRepository receiptProductRepository)
        {
            _productRepository = productRepository;
            _receiptRepository = receiptRepository;
            _receiptProductRepository = receiptProductRepository;
        }
        /// <summary>
        /// Obtiene los productos del carrito de compras.
        /// </summary>
        /// <returns>Productos en el carrito</returns>
        /// <response code="200">Los productos en el carrito</response>
        [HttpGet]
        public IActionResult GetProducts()
        {
            var userGuid = GetOrCreateUserGuid();
            var products = GetProductsFromCookies(userGuid);
            return Ok(products);
        }
        /// <summary>
        /// Agrega un producto al carrito de compras.
        /// </summary>
        /// <param name="newProductID">El producto a agregar al carrito</param>
        /// <returns></returns>
        /// <response code="200">Producto agregado al carrito</response>
        /// <response code="400">Producto sin stock o no es válido</response>
        [HttpPost("{newProductID}")]
        public async Task<IActionResult> AddProduct(int newProductID)
        {
            var userGuid = GetOrCreateUserGuid();

            var products = GetProductsFromCookies(userGuid);

            var product = await _productRepository.GetById(newProductID);
            if (product == null)
            {
                return NotFound("Product not found");
            }
            // En caso de que se ingrese un cáracter no numérico
            if (product == null)
            {
                return NotFound("Product not found");
            }
            if (product.Stock == 0)
            {
                return BadRequest("Product out of stock");
            }
            var newProductDTO = product.ToProductInCartDTO();
            if (products.Any(p => p.ID == newProductDTO.ID))
            {
                products.First(p => p.ID == newProductDTO.ID).Quantity++;
            }
            else
            {
                newProductDTO.Quantity = 1;
                products.Add(newProductDTO);
            }

            SaveProductsToCookies(userGuid, products);
            return Ok("Product added to cart");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Id del producto a actualizar</param>
        /// <param name="addOrDecrease">Se añade o quita cantidad de producto en el carrito</param>
        /// <returns>Producto actualizado</returns>
        /// <response code="200">Producto actualizado correctamente</response>
        /// <response code="400">Cantidad no puede ser menor a 1, mal ingreso de parámetros o el producto no fue encontrado</response>
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, string addOrDecrease)
        {
            var userGuid = GetOrCreateUserGuid();

            var products = GetProductsFromCookies(userGuid);

            var existingProduct = products.FirstOrDefault(p => p.ID == id);

            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }
            if (addOrDecrease == "add")
            {
                existingProduct.Quantity++;
            }
            else if (addOrDecrease == "decrease")
            {
                if (existingProduct.Quantity > 1)
                {
                    existingProduct.Quantity--;
                }
                else
                {
                    return BadRequest("Quantity can't be less than 1");
                }
            }
            else
            {
                return BadRequest("You should type 'add' or 'decrease'");
            }
            SaveProductsToCookies(userGuid, products);

            return Ok(existingProduct);
        }
        /// <summary>
        /// Elimina un producto del carrito de compras.
        /// </summary>
        /// <param name="id">Id del producto a eliminar</param>
        /// <returns></returns>
        /// <response code="200">Producto eliminado del carrito de manera correcta</response>
        /// <response code="404">Producto no encontrado</response>
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var userGuid = GetOrCreateUserGuid();

            var products = GetProductsFromCookies(userGuid);

            var productToRemove = products.FirstOrDefault(p => p.ID == id);

            if (productToRemove == null)
            {
                return NotFound("Product not found");
            }

            // Eliminar el producto de la lista de products
            products.Remove(productToRemove);

            // Guardar la lista de products en las cookies
            SaveProductsToCookies(userGuid, products);

            return Ok("Product removed from cart");
        }
        /// <summary>
        /// Compra los productos del carrito.
        /// </summary>
        /// <param name="addressDTO">Direcciones de entrega de los productos comprados</param>
        /// <returns>La boleta</returns>
        /// <response code="200">Productos comprados correctamente</response>
        /// <response code="400">Error al comprar los productos</response>
        [HttpPost("buy")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> BuyProducts(AddressDTO addressDTO)
        {
            try{
                var userGuid = GetOrCreateUserGuid();
                var products = GetProductsFromCookies(userGuid);
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var receipt = await _receiptRepository.CreateReceipt(products, userId, addressDTO);
                if (receipt != null)
                {
                    var receiptProducts = await _receiptProductRepository.AddReceiptProduct(products, receipt.Id);
                    if (receiptProducts != null)
                    {
                        var boughtReceipt = await _receiptRepository.GetById(receipt.Id);
                        var response = new
                        {
                            Message = "Products bought successfully",
                            Receipt = boughtReceipt,
                        };
                        return Ok(response);
                    }
                }
                return BadRequest("Error buying products");
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
        /// <summary>
        /// Crea u obtiene el GUID del usuario.
        /// </summary>
        /// <returns>Retorna el GUID</returns>
        private string GetOrCreateUserGuid()
        {
            var userGuid = Request.Cookies[UserCookieKey];

            if (string.IsNullOrEmpty(userGuid))
            {
                userGuid = Guid.NewGuid().ToString();
                var cookieOptions = new CookieOptions
                {
                    Path = "/",
                    HttpOnly = false,
                    Secure = false,
                    Expires = DateTime.Now.AddDays(7)
                };
                Response.Cookies.Append(UserCookieKey, userGuid, cookieOptions);
            }

            return userGuid;
        }
        /// <summary>
        /// Obtiene los productos del carrito de las cookies del usuario.
        /// </summary>
        /// <param name="userGuid">GUID del usuario</param>
        /// <returns>JSON de productos del carrito</returns>
        private List<ProductInCartDTO> GetProductsFromCookies(string userGuid)
        {
            var cookieValue = Request.Cookies[ProductCookieKey + "_" + userGuid];
            if (!string.IsNullOrEmpty(cookieValue))
            {
                return JsonSerializer.Deserialize<List<ProductInCartDTO>>(cookieValue) ?? [];
            }
            return [];
        }
        /// <summary>
        /// Guarda los productos del carrito en las cookies del usuario.
        /// </summary>
        /// <param name="userGuid">GUID del usuario</param>
        /// <param name="products">Lista de productos</param>
        private void SaveProductsToCookies(string userGuid, List<ProductInCartDTO> products)
        {
            var serializedProducts = JsonSerializer.Serialize(products);

            var cookieOptions = new CookieOptions
            {
                Path = "/",
                HttpOnly = false,
                Secure = false,
                Expires = DateTime.Now.AddDays(7)
            };

            Response.Cookies.Append(
                ProductCookieKey + "_" + userGuid,
                serializedProducts,
                cookieOptions
            );
        }
    }
}