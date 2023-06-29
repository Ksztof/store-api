using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.Forms;
using PerfumeStore.Core.Services.ProductsService;

namespace PerfumeStore.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ProductsController : ControllerBase
	{
		private readonly IProductsService _productService;
		public ProductsController(IProductsService productService)
		{
			_productService = productService;
		}


		[HttpPost]
		public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductForm createProductForm) 
		{
			throw new NotImplementedException();
		}


		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductForm updateform)
		{
			throw new NotImplementedException();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProductAsync(int id)
		{
			throw new NotImplementedException();
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetProductByIdAsync(int id)
		{
			throw new NotImplementedException();
		}

		[HttpGet]
		public async Task<IActionResult> GetAllProductsAsync()
		{
			throw new NotImplementedException();
		}


	}
}
