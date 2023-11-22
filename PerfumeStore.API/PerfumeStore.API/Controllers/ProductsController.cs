using Castle.Core.Internal;
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
            Result<ProductResponse> result = await _productService.CreateProductAsync(createProductForm);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }

        [HttpDelete("{productId}")]
        [Authorize(Roles.Administrator)]
        public async Task<IActionResult> DeleteProductAsync(int productId)
        {
            var result = await _productService.DeleteProductAsync(productId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductByIdAsync(int productId)
        {
            Result<ProductResponse> result = await _productService.GetProductByIdAsync(productId);
            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Entity);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProductsAsync()
        {
            IEnumerable<ProductResponse> result = await _productService.GetAllProductsAsync();
            if (!result.Any())
                return Ok("The list of products is empty");

            return Ok(result);
        }

        [HttpPut("{productId}")]
        [Authorize(Roles.Administrator)]
        public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductForm updateform, int productId)
        {
            Result<ProductResponse> result = await _productService.UpdateProductAsync(updateform, productId);
            if (result.IsFailure)
            {
                return NotFound(result.Error);
            }

            return Ok(result.Entity);
        }
    }
}