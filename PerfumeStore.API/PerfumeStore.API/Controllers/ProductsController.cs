using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.Forms;
using PerfumeStore.Core.Services.ProductsService;
using PerfumeStore.Domain.Models;

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
			int createdProductId = await _productService.CreateProductAsync(createProductForm);
			return CreatedAtAction(nameof(GetProductByIdAsync), new { id = createdProductId}, createdProductId);
		}

		[HttpPut]
		public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductForm updateform)
		{
			int updatedProductId = await _productService.UpdateProductAsync(updateform);
			return Ok(updatedProductId);
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProductAsync(int id)
		{
			int deletedProductId = await _productService.DeleteProductAsync(id);
			return NoContent();
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetProductByIdAsync(int id)
		{
			Products product = await _productService.GetProductByIdAsync(id);
			return Ok(product);
		}

		[HttpGet]
		public async Task<IActionResult> GetAllProductsAsync()
		{
			IEnumerable<Products> productsList = await _productService.GetAllProductsAsync();
			return Ok(productsList);
		}
	}
}
