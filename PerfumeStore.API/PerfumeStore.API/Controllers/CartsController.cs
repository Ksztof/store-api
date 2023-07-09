using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PerfumeStore.Core.Services;

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


		[HttpPost("{productId}/AddProductToCartAsync")]
		public async Task<IActionResult> AddProductToCartAsync(int productId)
		{
			int cartId = _cartsService.AddProductToCartAsync(productId);
			return CreatedAt
		}

		[HttpDelete("{productId}/RemoveProductLineFromCartAsync")]
		public async Task<IActionResult> RemoveProductLineFromCartAsync(int productId)
		{
			throw new NotImplementedException();
		}

		[HttpPost("{productId}/DecreaseProductQuantityAsync")]
		public async Task<IActionResult> DecreaseProductQuantityAsync(int productId)
		{
			throw new NotImplementedException();
		}

		[HttpPost("{productId}/IncreaseProductQuantityAsync")]
		public async Task<IActionResult> IncreaseProductQuantityAsync(int productId)
		{
			throw new NotImplementedException();
		}

		[HttpPost("{productId}/{productQuantity}/SetProductQuantityAsync")]
		public async Task<IActionResult> SetProductQuantityAsync(int productId, int productQuantity)
		{
			throw new NotImplementedException();
		}

		[HttpGet]
		public async Task<IActionResult> CheckCartAsync()
		{
			//Total cart value, List of products with ProductId, Unit price, Quantity, Total price
			throw new NotImplementedException();
		}

		[HttpDelete]
		public async Task<IActionResult> ClearCartAsync(int productId)
		{
			throw new NotImplementedException();
		}
	}
}
