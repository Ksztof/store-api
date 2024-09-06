using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Shared.DTO.Request.Product;
using Store.API.Shared.Extensions.Models;
using Store.API.Shared.Extensions.Results;
using Store.API.Validation;
using Store.Application.Products;
using Store.Application.Products.Dto.Request;
using Store.Application.Products.Dto.Response;
using Store.Domain.Abstractions;
using Store.Domain.Products;
using Store.Domain.StoreUsers.Roles;

namespace Store.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _productService;
    private readonly IMapper _mapper;
    private readonly IValidationService _validationService;

    public ProductsController(
        IProductsService productService,
        IMapper mapper,
        IValidationService validationService)
    {
        _productService = productService;
        _mapper = mapper;
        _validationService = validationService;
    }

    [HttpPost]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> CreateProductAsync([FromBody] CreateProductDtoApi createProductForm)
    {
        var validationResult = await _validationService.ValidateAsync(createProductForm);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblemDetails();
        }

        CreateProductDtoApp createProductDtoApp = _mapper.Map<CreateProductDtoApp>(createProductForm);

        EntityResult<ProductResponse> result = await _productService.CreateProductAsync(createProductDtoApp);

        CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetAllProductsAsync), new { productId = result.Entity?.Id }, result.Entity);

        return result.IsSuccess ? creationResult : result.ToProblemDetails();
    }

    [HttpDelete("{productId}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> DeleteProductAsync(int productId)
    {
        EntityResult<Product> result = await _productService.DeleteProductAsync(productId);

        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }

    [HttpGet("{productId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductByIdAsync(int productId)
    {
        EntityResult<ProductResponse> result = await _productService.GetProductByIdAsync(productId);

        return result.IsSuccess ? Ok(result.Entity) : result.ToProblemDetails();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllProductsAsync()
    {
        IEnumerable<ProductResponse> result = await _productService.GetAllProductsAsync();

        return Ok(result);
    }

    [HttpPut]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> UpdateProductAsync([FromBody] UpdateProductDtoApi updateProductForm)
    {
        var validationResult = await _validationService.ValidateAsync(updateProductForm);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblemDetails();
        }

        UpdateProductDtoApp updateProductDtoApp = _mapper.Map<UpdateProductDtoApp>(updateProductForm);

        EntityResult<ProductResponse> result = await _productService.UpdateProductAsync(updateProductDtoApp);

        CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetProductByIdAsync), new { productId = result.Entity?.Id }, result.Entity);

        return result.IsSuccess ? creationResult : result.ToProblemDetails();
    }
}