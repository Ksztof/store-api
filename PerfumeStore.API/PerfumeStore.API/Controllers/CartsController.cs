using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using PerfumeStore.Core.Services.Carts;

namespace PerfumeStore.API.Controllers
{
	[Route("api/[cotroller]")]
	[ApiController]
	public class CartsController : ControllerBase
	{
		private readonly ICartsService _cartsService;
		public CartsController(ICartsService cartsService)
		{
			_cartsService = cartsService;
		}


		[HttpPost("{productId}")]
		public async Task<IActionResult> AddProductToCart(int productId)
		{
			throw new NotImplementedException();
		}

		[HttpDelete("{productId}")]
		public async Task<IActionResult> RemoveProductLineFromCart(int productId)
		{
			throw new NotImplementedException();
		}

		[HttpPost("{productId}")]
		public async Task<IActionResult> DecreaseProductQuantity(int productId)
		{
			throw new NotImplementedException();
		}

		[HttpPost("{productId}")]
		public async Task<IActionResult> IncreaseProductQuantity(int productId)
		{
			throw new NotImplementedException();
		}

		[HttpPost("{productId}/{productQuantity}")]
		public async Task<IActionResult> SetProductQuantity(int productId, int productQuantity)
		{
			throw new NotImplementedException();
		}

		[HttpGet]
		public async Task<IActionResult> CheckCart()
		{
			//Total cart value, List of products with ProductId, Unit price, Quantity, Total price
			throw new NotImplementedException();
		}

		[HttpDelete]
		public async Task<IActionResult> ClearCart(int productId)
		{
			throw new NotImplementedException();
		}
	}
}
