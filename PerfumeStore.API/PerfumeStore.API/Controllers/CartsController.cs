using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Services;
using PerfumeStore.Domain.DbModels;
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

        [HttpPost("AddProductToCartAsync/{productId}/{productQuantity}")]
        public async Task<IActionResult> AddProductToCartAsync(int productId, decimal productQuantity)
        {
            CartDto cart = await _cartsService.AddProductToCartAsync(productId, productQuantity);
            return CreatedAtAction(nameof(GetCartByIdAsync), new { cartId = cart.Id }, cart);
        }

        [HttpPut("DeleteCartLineFromCart/{productId}/")]
        public async Task<IActionResult> DeleteProductLineFromCartAsync(int productId)
        {
            CartDto updatedCart = await _cartsService.DeleteCartLineFromCartAsync(productId);
            return Ok(updatedCart);
        }

        [HttpPost("SetProductQuantityAsync/{productId}/{productQuantity}/")]
        public async Task<IActionResult> SetProductQuantityAsync(int productId, decimal productQuantity)
        {
            Cart updatedCart = await _cartsService.SetProductQuantityAsync(productId, productQuantity);
            return Ok(updatedCart);
        }

        [HttpGet]
        public async Task<IActionResult> CheckCartAsync()
        {
            CheckCartForm products = await _cartsService.CheckCartAsync();
            return Ok(products);
        }

        [HttpDelete("ClearCartAsync/")]
        public async Task<IActionResult> ClearCartAsync()
        {
            Cart updatedCart = await _cartsService.ClearCartAsync();
            return Ok(updatedCart);
        }

        [HttpGet("GetCartByIdAsync/{cartId}")]
        public async Task<IActionResult> GetCartByIdAsync(int cartId)
        {
            Cart cart = await _cartsService.GetCartByIdAsync(cartId);
            return Ok(cart);
        }
    }
}