using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.DTOs.Request;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Application.Products;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.EnumsEtc;
using PerfumeStore.Domain.Products;

namespace PerfumeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductsService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDtoApi createProductForm)
        {
            CreateProductDtoApp createProductDtoApp = _mapper.Map<CreateProductDtoApp>(createProductForm);

            EntityResult<ProductResponse> result = await _productService.CreateProductAsync(createProductDtoApp);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetProductById), new { productId = result.Entity.Id }, result.Entity);
        }

        [HttpDelete("{productId}")]
        //[Authorize(Roles.Administrator)]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            EntityResult<Product> result = await _productService.DeleteProductAsync(productId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            EntityResult<ProductResponse> result = await _productService.GetProductByIdAsync(productId);

            if (result.IsFailure)
                return NotFound(result.Error);

            return Ok(result.Entity);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllProducts()
        {
            IEnumerable<ProductResponse> result = await _productService.GetAllProductsAsync();

            if (!result.Any())
                return Ok("The list of products is empty");

            return Ok(result);
        }

        [HttpPut]
        //[Authorize(Roles.Administrator)]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDtoApi updateProductForm)
        {
            UpdateProductDtoApp updateProductDtoApp = _mapper.Map<UpdateProductDtoApp>(updateProductForm);

            EntityResult<ProductResponse> result = await _productService.UpdateProductAsync(updateProductDtoApp);

            if (result.IsFailure)
                return NotFound(result.Error);

            return CreatedAtAction(nameof(GetProductById), new { productId = result.Entity.Id }, result.Entity);
        }
    }
}