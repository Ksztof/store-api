using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.DTOs.Request;
using PerfumeStore.Application.Carts;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Core.DTO;

namespace PerfumeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartsService _cartsService;

        public CartsController(ICartsService cartsService)
        {
            _cartsService = cartsService;
        }

        //public class AddProductsToCartRequest
        //{
        //    public ProductInCart[] Products { get; set; }
        //}

        //public class ProductInCart
        //{
        //    public int ProductId { get; set; }

        //    public int Quantity { get; set; }
        //}

        //public async Task<IActionResult> AddProductsToCartAsync([FromBody] AddProductsToCartRequest request)
        //{
        //    return null;
        //}

        [HttpPost("products/{productId}")] //KM Według mnie trochę się ograniczasz mają endpoint do jednego produktu
        public async Task<IActionResult> AddProductToCartAsync(int productId, [FromBody] QuantityRequest productQuantity)
        {
            CartResponse cart = await _cartsService.AddProductToCartAsync(productId, productQuantity.Quantity);
            if (cart.Errors is not null)
            {
                return BadRequest(cart.Errors);
            }

            return CreatedAtAction("GetCartById", new { cartId = cart.Id }, cart);
        }

        [HttpPut("products/{productId}")]
        public async Task<IActionResult> DeleteProductLineFromCartAsync(int productId)
        {
            CartResponse updatedCart = await _cartsService.DeleteCartLineFromCartAsync(productId);
            return Ok(updatedCart);
        }

        [HttpPut("products/{productId}/quantity")] //KM quantity jest tutaj zbędne, lepiej dać bez tego i jako klasy FromBody użyć ProductInCartRequest i dzięki temu jeśli keidyś pojawi Ci sie inny parametr do edycji to obsłużysz to tutaj
                                                   // W tej chwili każda modyfikacja produktu w koszyku to będzie osobna metoda
        public async Task<IActionResult> SetProductQuantityAsync(int productId, [FromBody] QuantityRequest productQuantity)
        {
            CartResponse updatedCart = await _cartsService.SetProductQuantityAsync(productId, productQuantity.Quantity);
            if (updatedCart.Errors is not null)
            {
                return BadRequest(updatedCart.Errors);
            }

            return Ok(updatedCart);
        }

        [HttpGet]
        public async Task<IActionResult> CheckCartAsync()
        {
            AboutCartRes products = await _cartsService.CheckCartAsync();
            return Ok(products);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCartAsync()
        {
            CartResponse updatedCart = await _cartsService.ClearCartAsync();
            return Ok(updatedCart);
        }

        [HttpGet("{cartId}", Name = "GetCartById")]
        public async Task<IActionResult> GetCartByIdAsync(int cartId)
        {
            CartResponse cart = await _cartsService.GetCartResponseByIdAsync(cartId);
            return Ok(cart);
        }
    }


}