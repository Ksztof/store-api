using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Shared.DTO.Request.Cart;
using Store.API.Shared.Extensions.Models;
using Store.API.Shared.Extensions.Results;
using Store.API.Validation;
using Store.Application.Carts;
using Store.Application.Carts.Dto.Request;
using Store.Application.Carts.Dto.Response;
using Store.Domain.Abstractions;
using Store.Domain.Carts.Dto.Response;
using Store.Domain.StoreUsers.Roles;

namespace Store.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartsController : ControllerBase
{
    private readonly ICartsService _cartsService;
    private readonly IMapper _mapper;
    private readonly IValidationService _validationService;

    public CartsController(
        ICartsService cartsService,
        IMapper mapper,
        IValidationService validationService)
    {
        _cartsService = cartsService;
        _mapper = mapper;
        _validationService = validationService;
    }

    [HttpPost("products")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> AddProductsToCartAsync([FromBody] NewProductsDtoApi request)
    {
        var validationResult = await _validationService.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblemDetails();
        }

        NewProductsDtoApp addProductToCartDto = _mapper.Map<NewProductsDtoApp>(request);

        EntityResult<CartResponse> result = await _cartsService.AddProductsToCartAsync(addProductToCartDto);

        CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetCartByIdAsync), new { cartId = result.Entity?.CartId }, result.Entity);

        return result.IsSuccess ? creationResult : result.ToProblemDetails();
    }

    [HttpDelete("products/{productId}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> DeleteProductFromCartAsync(int productId)
    {
        EntityResult<CartResponse> result = await _cartsService.DeleteProductFromCartAsync(productId);

        CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetCartByIdAsync), new { cartId = result.Entity?.CartId }, result.Entity);

        return result.IsSuccess ? creationResult : result.ToProblemDetails();
    }

    [HttpPatch("products")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> ModifyProductAsync([FromBody] ModifyProductDtoApi modifiedProduct)
    {
        var validationResult = await _validationService.ValidateAsync(modifiedProduct);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblemDetails();
        }

        ModifyProductDtoApp modifyProductDto = _mapper.Map<ModifyProductDtoApp>(modifiedProduct);

        EntityResult<CartResponse> result = await _cartsService.ModifyProductAsync(modifyProductDto);

        CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetCartByIdAsync), new { cartId = result.Entity?.CartId }, result.Entity);

        return result.IsSuccess ? creationResult : result.ToProblemDetails();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> CheckCartAsync()
    {
        EntityResult<AboutCartDomRes> result = await _cartsService.CheckCartAsync();

        if (result.IsSuccess && result.Entity == null)
        {
            return NoContent();
        }

        if (result.IsFailure)
        {
            return result.ToProblemDetails();
        }

        return Ok(result.Entity);
    }

    [HttpDelete]
    [AllowAnonymous]
    public async Task<IActionResult> ClearCartAsync()
    {
        Result result = await _cartsService.ClearCartAsync();

        return result.IsSuccess ? NoContent() : result.ToProblemDetails();
    }

    [HttpGet("{cartId}")]
    [Authorize(Roles = UserRoles.Administrator)]
    public async Task<IActionResult> GetCartByIdAsync(int cartId)
    {
        EntityResult<CartResponse> result = await _cartsService.GetCartByIdAsync(cartId);

        return result.IsSuccess ? Ok(result.Entity) : result.ToProblemDetails();
    }

    [HttpPut]
    [AllowAnonymous]
    public async Task<IActionResult> ReplaceCartContentAsync(NewProductsDtoApi request)
    {
        var validationResult = await _validationService.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblemDetails();
        }

        NewProductsDtoApp addProductToCartDto = _mapper.Map<NewProductsDtoApp>(request);

        EntityResult<AboutCartDomRes> result = await _cartsService.ReplaceCartContentAsync(addProductToCartDto);

        if (result.IsSuccess && result.Entity == null)
        {
            return NoContent();
        }

        if (result.IsFailure)
        {
            return result.ToProblemDetails();
        }

        return Ok(result.Entity);
    }

    [HttpPost("check-current-cart")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckCurrentCartAsync([FromBody] CheckCurrentCartDtoApi request)
    {
        var validationResult = await _validationService.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return validationResult.ToValidationProblemDetails();
        }

        CheckCurrentCartDtoApp addProductToCartDto = _mapper.Map<CheckCurrentCartDtoApp>(request);

        EntityResult<AboutCartDomRes> result = await _cartsService.CheckCurrentCartAsync(addProductToCartDto);

        if (result.IsFailure)
        {
            return result.ToProblemDetails();
        }

        if (result.Entity == null)
        {
            return NoContent();
        }

        return Ok(result.Entity);
    }
}