﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.Forms;
using PerfumeStore.Core.Services;
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
			Product updatedProductId = await _productService.UpdateProductAsync(updateform);
			return Ok(updatedProductId);
		}

		[HttpDelete("{productId}")]
		public async Task<IActionResult> DeleteProductAsync(int productId)
		{
			await _productService.DeleteProductAsync(productId);
			return NoContent();
		}

		[HttpGet("{productId}")]
		public async Task<IActionResult> GetProductByIdAsync(int productId)
		{
			Product product = await _productService.GetProductByIdAsync(productId);
			return Ok(product);
		}

		[HttpGet]
		public async Task<IActionResult> GetAllProductsAsync()
		{
			IEnumerable<Product> products = await _productService.GetAllProductsAsync();
			return Ok(products);
		}
	}
}