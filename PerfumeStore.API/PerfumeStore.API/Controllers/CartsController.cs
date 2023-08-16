using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.DTOs.Request;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Services;
using PerfumeStore.Domain.Models;

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

        [HttpPost("products/{productId}")]
        public async Task<IActionResult> AddProductToCartAsync(int productId, [FromBody] QuantityRequest productQuantity)
        {
            CartResponse cart = await _cartsService.AddProductToCartAsync(productId, productQuantity.Quantity);
            return CreatedAtAction("GetCartById", new { cartId = cart.Id }, cart);
        }

        [HttpPut("products/{productId}")]
        public async Task<IActionResult> DeleteProductLineFromCartAsync(int productId)
        {
            CartResponse updatedCart = await _cartsService.DeleteCartLineFromCartAsync(productId);
            return Ok(updatedCart);
        }

        [HttpPost("products/{productId}/quantity")]
        public async Task<IActionResult> SetProductQuantityAsync(int productId, [FromBody] QuantityRequest productQuantity)
        {
            CartResponse updatedCart = await _cartsService.SetProductQuantityAsync(productId, productQuantity.Quantity);
            return Ok(updatedCart);
        }

        [HttpGet]
        public async Task<IActionResult> CheckCartAsync()
        {
            AboutCartResponse products = await _cartsService.CheckCartAsync();
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
            CartResponse cart = await _cartsService.GetCartByIdAsync(cartId);
            return Ok(cart);
        }
    }
}