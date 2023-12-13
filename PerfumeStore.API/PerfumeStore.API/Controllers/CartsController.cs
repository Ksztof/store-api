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
        public async Task<IActionResult> AddProductToCartAsync([FromBody] AddProductsToCartDtoApi request)
        {
            AddProductsToCartDtoApp addProductToCartDto = _mapper.Map<AddProductsToCartDtoApp>(request);

            EntityResult<CartResponse> result = await _cartsService.AddProductsToCartAsync(addProductToCartDto);
            if (result.IsFailure)
            {
                return BadRequest(result.Error);
            }

            return CreatedAtAction("GetCartById", new { cartId = result.Entity.Id }, result.Entity);
        }

        [HttpPut("products/{productId}")]
        public async Task<IActionResult> DeleteProductLineFromCartAsync(int productId)
        {
            EntityResult<CartResponse> result = await _cartsService.DeleteCartLineFromCartAsync(productId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }

        [HttpPut("products")]
        public async Task<IActionResult> ModifyProductAsync([FromBody] ModifyProductDtoApi modifiedProduct)
        {
            ModifyProductDtoApp modifyProductDto = _mapper.Map<ModifyProductDtoApp>(modifiedProduct);

            EntityResult<CartResponse> result = await _cartsService.ModifyProductAsync(modifyProductDto);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CheckCartAsync()
        {
            EntityResult<AboutCartRes> result = await _cartsService.CheckCartAsync();
            if (result.IsSuccess && result.Entity == null)
            {
                return Ok("Cart is empty");

            }

            return Ok(result.Entity);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCartAsync()
        {
            EntityResult<CartResponse> result = await _cartsService.ClearCartAsync();
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }

        [HttpGet("{cartId}", Name = "GetCartById")]
        public async Task<IActionResult> GetCartByIdAsync(int cartId)
        {
            EntityResult<CartResponse> result = await _cartsService.GetCartResponseByIdAsync(cartId);
            if (result.IsFailure)
                return BadRequest(result.Error);

            return Ok(result.Entity);
        }
    }


}