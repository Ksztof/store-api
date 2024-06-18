using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using PerfumeStore.API.Shared.DTO.Request.Product;
using PerfumeStore.API.Shared.Extensions;
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
        //[Authorize(Roles = Roles.Administrator)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDtoApi createProductForm)
        {
            var validationResult = await _validationService.ValidateAsync(createProductForm);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            CreateProductDtoApp createProductDtoApp = _mapper.Map<CreateProductDtoApp>(createProductForm);

            EntityResult<ProductResponse> result = await _productService.CreateProductAsync(createProductDtoApp);

            CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetProductById), new { productId = result.Entity.Id }, result.Entity);

            return result.IsSuccess ? creationResult : result.ToProblemDetails();
        }

        [HttpDelete("{productId}")]
        //[Authorize(Roles.Administrator)]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            EntityResult<Product> result = await _productService.DeleteProductAsync(productId);

            return result.IsSuccess ? NoContent() : result.ToProblemDetails();
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            EntityResult<ProductResponse> result = await _productService.GetProductByIdAsync(productId);

            return result.IsSuccess ? Ok(result.Entity) : result.ToProblemDetails();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            IEnumerable<ProductResponse> result = await _productService.GetAllProductsAsync();

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

            CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetProductById), new { productId = result.Entity.Id }, result.Entity);

            return result.IsSuccess ? creationResult : result.ToProblemDetails();
        }
    }
}