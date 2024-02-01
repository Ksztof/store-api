using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using PerfumeStore.API.Shared.DTO.Request.Product;
using PerfumeStore.API.Validators;
using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Products;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.Entities.Products;
using PerfumeStore.Domain.Shared;

namespace PerfumeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductsService _productService;
        private readonly IMapper _mapper;
        private readonly IValidationService _validationService;

        public ProductsController(IProductsService productService, IMapper mapper, IValidationService validationService)
        {
            _productService = productService;
            _mapper = mapper;
            _validationService = validationService;
        }

        [HttpPost]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDtoApi createProductForm)
        {
            var validationResult = await _validationService.ValidateAsync(createProductForm);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

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
            if (productId <= 0)
                return BadRequest("Wrong product id");

            EntityResult<Product> result = await _productService.DeleteProductAsync(productId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return NoContent();
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            if (productId <= 0)
                return BadRequest("Wrong product id");

            EntityResult<ProductResponse> result = await _productService.GetProductByIdAsync(productId);

            if (result.IsFailure)
                return NotFound(result.Error);

            return Ok(result.Entity);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            IEnumerable<ProductResponse> result = await _productService.GetAllProductsAsync();

            if (!result.Any())
                return Ok("The list of products is empty");
           
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDtoApi updateProductForm)
        {
            var validationResult = await _validationService.ValidateAsync(updateProductForm);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            UpdateProductDtoApp updateProductDtoApp = _mapper.Map<UpdateProductDtoApp>(updateProductForm);

            EntityResult<ProductResponse> result = await _productService.UpdateProductAsync(updateProductDtoApp);

            if (result.IsFailure)
                return NotFound(result.Error);

            return CreatedAtAction(nameof(GetProductById), new { productId = result.Entity.Id }, result.Entity);
        }
    }
}