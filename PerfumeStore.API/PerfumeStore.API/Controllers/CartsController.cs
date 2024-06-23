using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using PerfumeStore.API.Shared.DTO.Request.Cart;
using PerfumeStore.API.Shared.Extensions;
using PerfumeStore.API.Validators;
using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Carts;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.DTO.Response.Cart;
using PerfumeStore.Domain.Entities.Carts;
using PerfumeStore.Domain.Shared;

namespace PerfumeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<IActionResult> AddProductsToCartAsync([FromBody] NewProductsDtoApi request)
        {
            var validationResult = await _validationService.ValidateAsync(request);

            if (!validationResult.IsValid)
                return validationResult.ToValidationProblemDetails();


            NewProductsDtoApp addProductToCartDto = _mapper.Map<NewProductsDtoApp>(request);

            EntityResult<CartResponse> result = await _cartsService.AddProductsToCartAsync(addProductToCartDto);

            CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.CartId }, result.Entity);

            return result.IsSuccess ? creationResult : result.ToProblemDetails();
        }

        [HttpDelete("products/{productId}")]
        public async Task<IActionResult> DeleteProductFromCartAsync(int productId)
        {
            EntityResult<CartResponse> result = await _cartsService.DeleteCartLineFromCartAsync(productId);

            CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.CartId }, result.Entity);

            return result.IsSuccess ? creationResult : result.ToProblemDetails();
        }

        [HttpPatch("products")]
        public async Task<IActionResult> ModifyProductAsync([FromBody] ModifyProductDtoApi modifiedProduct)
        {
            var validationResult = await _validationService.ValidateAsync(modifiedProduct);

            if (!validationResult.IsValid)
                return validationResult.ToValidationProblemDetails();


            ModifyProductDtoApp modifyProductDto = _mapper.Map<ModifyProductDtoApp>(modifiedProduct);

            EntityResult<CartResponse> result = await _cartsService.ModifyProductAsync(modifyProductDto);

            CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.CartId }, result.Entity);

            return result.IsSuccess ? creationResult : result.ToProblemDetails();
        }

        [HttpGet]
        public async Task<IActionResult> CheckCartAsync()
        {
            EntityResult<AboutCartDomRes> result = await _cartsService.CheckCartAsync();

            if (result.IsSuccess && result.Entity == null)
                return NoContent();

            if (result.IsFailure)
                return result.ToProblemDetails();

            return Ok(result.Entity);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            EntityResult<CartResponse> result = await _cartsService.ClearCartAsync();

            CreatedAtActionResult creationResult = CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.CartId }, result.Entity);

            return result.IsSuccess ? creationResult : result.ToProblemDetails();
        }

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartById(int cartId)
        {
            EntityResult<CartResponse> result = await _cartsService.GetCartResponseByIdAsync(cartId);

            return result.IsSuccess ? Ok(result.Entity) : result.ToProblemDetails();
        }

        [HttpPut]
        public async Task<IActionResult> ReplaceCartContentAsync(NewProductsDtoApi request)
        {
            var validationResult = await _validationService.ValidateAsync(request);

            if (!validationResult.IsValid)
                return validationResult.ToValidationProblemDetails();


            NewProductsDtoApp addProductToCartDto = _mapper.Map<NewProductsDtoApp>(request);

            EntityResult<AboutCartDomRes> result = await _cartsService.ReplaceCartContentAsync(addProductToCartDto);

            if (result.IsSuccess && result.Entity == null)
                return NoContent(); 

            if (result.IsFailure)
                return result.ToProblemDetails();

            return Ok(result.Entity);
        }

        [HttpPost("check-current-cart")]
        public async Task<IActionResult> CheckCurrentCartAsync([FromBody] CheckCurrentCartDtoApi request)
        {
            FluentValidation.Results.ValidationResult validationResult = await _validationService.ValidateAsync(request);

            if (!validationResult.IsValid)
                return validationResult.ToValidationProblemDetails();


            CheckCurrentCartDtoApp addProductToCartDto = _mapper.Map<CheckCurrentCartDtoApp>(request);

            EntityResult<AboutCartDomRes> result = await _cartsService.IsCurrentCartAsync(addProductToCartDto);

            if (result.IsFailure)
                return result.ToProblemDetails();

            if (result.Entity == null)
                return NoContent();

            return Ok(result.Entity);
        }
    }
}