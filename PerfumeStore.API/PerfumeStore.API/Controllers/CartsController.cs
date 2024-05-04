using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using PerfumeStore.API.Shared.DTO.Request.Cart;
using PerfumeStore.API.Validators;
using PerfumeStore.Application.Abstractions.Result.Entity;
using PerfumeStore.Application.Carts;
using PerfumeStore.Application.Shared.DTO.Request;
using PerfumeStore.Application.Shared.DTO.Response;
using PerfumeStore.Domain.DTO.Response.Cart;
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
            {
                return BadRequest(validationResult.Errors);
            }

            NewProductsDtoApp addProductToCartDto = _mapper.Map<NewProductsDtoApp>(request);

            EntityResult<CartResponse> result = await _cartsService.AddProductsToCartAsync(addProductToCartDto);
            
            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.CartId }, result.Entity);
        }

        [HttpDelete("products/{productId}")]
        public async Task<IActionResult> DeleteProductFromCartAsync(int productId)
        {
            if (productId <= 0)
                return BadRequest("Wrong product id");

            EntityResult<CartResponse> result = await _cartsService.DeleteCartLineFromCartAsync(productId);

            if (result.IsFailure)
            {
                var resultErr = new ObjectResult(result.Error)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };

                return resultErr;
            }

            return CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.CartId }, result.Entity);
        }

        [HttpPatch("products")]
        public async Task<IActionResult> ModifyProductAsync([FromBody] ModifyProductDtoApi modifiedProduct)
        {
            var validationResult = await _validationService.ValidateAsync(modifiedProduct);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            ModifyProductDtoApp modifyProductDto = _mapper.Map<ModifyProductDtoApp>(modifiedProduct);

            EntityResult<CartResponse> result = await _cartsService.ModifyProductAsync(modifyProductDto);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.CartId }, result.Entity);
        }

        [Authorize(Roles = Roles.Administrator)]
        [HttpGet]
        public async Task<IActionResult> CheckCartAsync()
        {
            var wad = HttpContext.Request.Cookies["AuthCookie"];
            EntityResult<AboutCartDomRes> result = await _cartsService.CheckCartAsync();

            if (result.IsSuccess && result.Entity == null)
                return NoContent();

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            EntityResult<CartResponse> result = await _cartsService.ClearCartAsync();

            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.CartId }, result.Entity);
        }

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartById(int cartId)
        {
            if (cartId <= 0)
                return BadRequest("Wrong cart id");

            EntityResult<CartResponse> result = await _cartsService.GetCartResponseByIdAsync(cartId);

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }

        [HttpPut]
        public async Task<IActionResult> ReplaceCartContentAsync(NewProductsDtoApi request)
        {
            var validationResult = await _validationService.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            NewProductsDtoApp addProductToCartDto = _mapper.Map<NewProductsDtoApp>(request);

            EntityResult<AboutCartDomRes> result = await _cartsService.ReplaceCartContentAsync(addProductToCartDto);

            if (result.IsSuccess && result.Entity == null)
                return Ok("Cart is empty");

            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }
    }
}