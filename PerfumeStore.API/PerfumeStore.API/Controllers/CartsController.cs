using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PerfumeStore.Core.ResponseForms;
using PerfumeStore.Core.Services;
using PerfumeStore.Domain.DbModels;

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
			Cart? cart = await _cartsService.AddProductToCartAsync(productId, productQuantity);
			return CreatedAtAction(nameof(GetCartByIdAsync), new { cartId = cart.CartId }, cart);
		}

		[HttpPut("DeleteProductLineFromCartAsync/{productId}/{userId}/")]
		public async Task<IActionResult> DeleteProductLineFromCartAsync(int productId, int userId)
		{
			Cart updatedCart = await _cartsService.DeleteProductLineFromCartAsync(productId, userId);
			return Ok(updatedCart);
		}

		[HttpPut("DecreaseProductQuantityAsync/{productId}/{userId}/")]
		public async Task<IActionResult> DecreaseProductQuantityAsync(int productId, int userId)
		{
			Cart updatedCart = await _cartsService.DecreaseProductQuantityAsync(productId, userId);
			return Ok(updatedCart);
		}

		[HttpPut("IncreaseProductQuantityAsync/{productId}/{userId}/")]
		public async Task<IActionResult> IncreaseProductQuantityAsync(int productId, int userId)
		{
			Cart updatedCart = await _cartsService.IncreaseProductQuantityAsync(productId, userId);
			return Ok(updatedCart);
		}

		[HttpPost("SetProductQuantityAsync/{productId}/{productQuantity}/{userId}/")]
		public async Task<IActionResult> SetProductQuantityAsync(int productId, decimal productQuantity, int userId)
		{
			Cart updatedCart = await _cartsService.SetProductQuantityAsync(productId, productQuantity, userId);
			return Ok(updatedCart);
		}

		[HttpGet("CheckCartAsync/{userId}")]
		public async Task<IActionResult> CheckCartAsync(int userId)
		{
			CheckCartForm products = await _cartsService.CheckCartAsync(userId);
			return Ok(products);
		}

		[HttpDelete("ClearCartAsync/{userId}")]
		public async Task<IActionResult> ClearCartAsync(int userId)
		{
			Cart updatedCart = await _cartsService.ClearCartAsync(userId);
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
