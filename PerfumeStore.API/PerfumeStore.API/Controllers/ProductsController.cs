using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Core.RequestForms;
using PerfumeStore.Core.Services;
using PerfumeStore.Domain.DbModels;

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

        [HttpPost("CreateProductAsync")]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductForm createProductForm)
        {
            Product createdProduct = await _productService.CreateProductAsync(createProductForm);
            return CreatedAtAction(nameof(GetProductByIdAsync), new { productId = createdProduct.ProductId }, createdProduct);
        }

        [HttpPut("UpdateProductAsync")]
        public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductForm updateform)
        {
            Product updatedProductId = await _productService.UpdateProductAsync(updateform);
            return Ok(updatedProductId);
        }

        [HttpDelete("DeleteProductAsync/{productId}")]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            await _productService.DeleteProductAsync(productId);
            return NoContent();
        }

        [HttpGet("GetProductByIdAsync/{productId}")]
        public async Task<IActionResult> GetProductByIdAsync(int productId)
        {
            Product product = await _productService.GetProductByIdAsync(productId);
            return Ok(product);
        }

        [HttpGet("GetAllProductsAsync")]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            IEnumerable<Product> products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
    }
}
