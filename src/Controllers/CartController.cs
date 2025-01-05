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
        [HttpGet]
        public IActionResult GetProducts()
        {
            var userGuid = GetOrCreateUserGuid();
            var products = GetProductsFromCookies(userGuid);
            return Ok(products);
        }
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
 [HttpPost("buy")]
[Authorize(Roles = "User")]
public async Task<IActionResult> BuyProducts(AddressDTO addressDTO)
{
    try
    {
        var userGuid = GetOrCreateUserGuid();
        var products = GetProductsFromCookies(userGuid);
        
        // Si no hay productos en el carrito, retornar un error
        if (products.Count == 0)
        {
            return BadRequest("No products in cart.");
        }

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var receipt = await _receiptRepository.CreateReceipt(products, userId, addressDTO);

        if (receipt != null)
        {
            var receiptProducts = await _receiptProductRepository.AddReceiptProduct(products, receipt.Id);
            if (receiptProducts != null)
            {
                // Crear la respuesta con los detalles de la compra
                var boughtReceipt = await _receiptRepository.GetById(receipt.Id);
                var response = new
                {
                    Message = "Products bought successfully",
                    Receipt = boughtReceipt,
                };

                // Vaciar la cookie de productos después de la compra
                SaveProductsToCookies(userGuid, new List<ProductInCartDTO>());

                // Devolver la respuesta con la boleta y mensaje
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
        private List<ProductInCartDTO> GetProductsFromCookies(string userGuid)
        {
            var cookieValue = Request.Cookies[ProductCookieKey + "_" + userGuid];
            if (!string.IsNullOrEmpty(cookieValue))
            {
                return JsonSerializer.Deserialize<List<ProductInCartDTO>>(cookieValue) ?? [];
            }
            return [];
        }
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