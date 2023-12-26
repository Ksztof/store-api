using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PerfumeStore.API.DTOs.Request;
using PerfumeStore.Application.Carts;
using PerfumeStore.Application.DTOs.Request;
using PerfumeStore.Application.DTOs.Response;
using PerfumeStore.Domain.Abstractions;
using PerfumeStore.Domain.Core.DTO;
using PerfumeStore.Domain.Products;

namespace PerfumeStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartsService _cartsService;
        private readonly IMapper _mapper;

        public CartsController(ICartsService cartsService, IMapper mapper)
        {
            _cartsService = cartsService;
            _mapper = mapper;
        }

        [HttpPost("products")]
        public async Task<IActionResult> AddProductsToCart([FromBody] AddProductsToCartDtoApi request)
        {
            AddProductsToCartDtoApp addProductToCartDto = _mapper.Map<AddProductsToCartDtoApp>(request);

            EntityResult<CartResponse> result = await _cartsService.AddProductsToCartAsync(addProductToCartDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.Id }, result.Entity);
        }

        [HttpDelete("products/{productId}")]
        public async Task<IActionResult> DeleteProductFromCart(int productId)
        {
            EntityResult<CartResponse> result = await _cartsService.DeleteCartLineFromCartAsync(productId);
            if (result.IsFailure)
            {
                var resultErr = new ObjectResult(result.Error)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };

                return resultErr;
            }

            return CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.Id }, result.Entity);
        }

        [HttpPatch("products")]
        public async Task<IActionResult> ModifyProduct([FromBody] ModifyProductDtoApi modifiedProduct)
        {
            ModifyProductDtoApp modifyProductDto = _mapper.Map<ModifyProductDtoApp>(modifiedProduct);

            EntityResult<CartResponse> result = await _cartsService.ModifyProductAsync(modifyProductDto);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.Id }, result.Entity);
        }

        [HttpGet]
        public async Task<IActionResult> CheckCart()
        {
            EntityResult<AboutCartRes> result = await _cartsService.CheckCartAsync();
            if (result.IsSuccess && result.Entity == null)
            {
                return Ok("Cart is empty");
            }

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

            return CreatedAtAction(nameof(GetCartById), new { cartId = result.Entity.Id }, result.Entity);
        }

        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartById(int cartId)
        {
            EntityResult<CartResponse> result = await _cartsService.GetCartResponseByIdAsync(cartId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }
    }


}