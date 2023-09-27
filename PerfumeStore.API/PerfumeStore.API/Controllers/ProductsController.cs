using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.DTOs.Response;
using PerfumeStore.Core.Services;
using PerfumeStore.Domain.Models;

namespace PerfumeStore.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  [Authorize]
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
      ProductResponse createdProduct = await _productService.CreateProductAsync(createProductForm);
      return Ok(createdProduct);
    }

    [HttpPut("{productId}")]
    public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductForm updateform, int productId)
    {
      ProductResponse updatedProductId = await _productService.UpdateProductAsync(updateform, productId);
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
      ProductResponse product = await _productService.GetProductByIdAsync(productId);
      return Ok(product);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProductsAsync()
    {
      IEnumerable<ProductResponse> products = await _productService.GetAllProductsAsync();
      return Ok(products);
    }
  }
}