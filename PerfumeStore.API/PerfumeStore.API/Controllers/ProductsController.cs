using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Application.Products;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.EnumsEtc;

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
        [Authorize(Roles.Administrator)]
        public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductForm createProductForm)
        {
            Result result = await _productService.CreateProductAsync(createProductForm);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok();// How to return Entity while success
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles.Administrator)]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            Result result = await _productService.DeleteProductAsync(productId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpPut("{productId}")]
        [Authorize(Roles.Administrator)]
        public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductForm updateform, int productId)
        {
            ProductResponse updatedProductId = await _productService.UpdateProductAsync(updateform, productId);

            return Ok(updatedProductId);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductByIdAsync(int productId)
        {
            ProductResponse product = await _productService.GetProductByIdAsync(productId);
            return Ok(product);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            IEnumerable<ProductResponse> products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }
    }
}