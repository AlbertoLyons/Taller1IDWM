using System.Text.Json;
using Taller_1_IDWM.src.Models;
using Microsoft.AspNetCore.Mvc;
namespace Taller_1_IDWM.src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private const string ProductCookieKey = "ProductsList";
        private const string UserCookieKey = "UserGUID";
        [HttpGet]
        public IActionResult GetProducts()
        {
            var userGuid = GetOrCreateUserGuid();
            var products = GetProductsFromCookies(userGuid);
            return Ok(products);
        }
        [HttpPost]
        public IActionResult AddProduct([FromBody] Product newProduct)
        {
            var userGuid = GetOrCreateUserGuid();

            var products = GetProductsFromCookies(userGuid);

            newProduct.ID = products.Count > 0 ? products.Max(p => p.ID) + 1 : 1;

            products.Add(newProduct);

            SaveProductsToCookies(userGuid, products);

            return Ok(newProduct);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
        {
            var userGuid = GetOrCreateUserGuid();

            var products = GetProductsFromCookies(userGuid);

            var existingProduct = products.FirstOrDefault(p => p.ID == id);

            if (existingProduct == null)
            {
                return NotFound("Product not found");
            }

            existingProduct.Name = updatedProduct.Name;
            existingProduct.Price = updatedProduct.Price;

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

            // Eliminar el ticket de la lista de products
            products.Remove(productToRemove);

            // Guardar la lista de products en las cookies
            SaveProductsToCookies(userGuid, products);

            return NoContent();
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
        private List<Product> GetProductsFromCookies(string userGuid)
        {
            var cookieValue = Request.Cookies[ProductCookieKey + "_" + userGuid];
            if (!string.IsNullOrEmpty(cookieValue))
            {
                return JsonSerializer.Deserialize<List<Product>>(cookieValue) ?? [];
            }
            return [];
        }
        private void SaveProductsToCookies(string userGuid, List<Product> products)
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